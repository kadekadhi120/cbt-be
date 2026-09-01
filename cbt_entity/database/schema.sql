-- =============================================================================
-- SmartCBT - PostgreSQL Database Schema
-- =============================================================================
-- Jalankan file ini secara berurutan:
--   1. schema.sql  (file ini)  → buat struktur tabel
--   2. seed.sql                → isi data awal / demo
-- =============================================================================

-- Pastikan extension uuid tersedia
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- =============================================================================
-- ENUM TYPES
-- =============================================================================

CREATE TYPE user_role    AS ENUM ('admin', 'student');
CREATE TYPE user_status  AS ENUM ('active', 'inactive');

CREATE TYPE exam_status  AS ENUM ('draft', 'published', 'closed');
CREATE TYPE question_type AS ENUM ('multiple_choice');

CREATE TYPE attempt_status AS ENUM (
  'in_progress',
  'submitted',
  'force_submitted',
  'time_expired'
);

CREATE TYPE submit_type AS ENUM (
  'manual',
  'auto_time_expired',
  'force_anticheat'
);

CREATE TYPE violation_type AS ENUM (
  'tab_switch',
  'window_blur',
  'force_submit'
);

CREATE TYPE activity_type AS ENUM (
  'info',
  'success',
  'warning',
  'danger'
);

-- =============================================================================
-- TABLE: users
-- =============================================================================
-- Menyimpan semua pengguna: admin dan siswa dalam satu tabel (single-table
-- inheritance) karena kolom hanya berbeda di `class` yang nullable.
-- =============================================================================

CREATE TABLE users (
  id           UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
  name         VARCHAR(150) NOT NULL,
  email        VARCHAR(255) NOT NULL,
  password_hash TEXT        NOT NULL,          -- bcrypt hash, minimal 60 char
  role         user_role   NOT NULL DEFAULT 'student',
  status       user_status NOT NULL DEFAULT 'active',
  class        VARCHAR(50),                    -- hanya diisi untuk siswa
  avatar_url   TEXT,
  created_at   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at   TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  CONSTRAINT users_email_unique UNIQUE (email),
  CONSTRAINT users_email_format CHECK (email ~* '^[^@\s]+@[^@\s]+\.[^@\s]+$')
);

CREATE INDEX idx_users_role   ON users (role);
CREATE INDEX idx_users_status ON users (status);
CREATE INDEX idx_users_email  ON users (email);

COMMENT ON TABLE  users            IS 'Semua pengguna platform: admin dan siswa.';
COMMENT ON COLUMN users.class      IS 'Kelas/kelompok siswa, contoh: XII IPA 1. NULL untuk admin.';
COMMENT ON COLUMN users.password_hash IS 'Hash bcrypt dari password. JANGAN simpan plain text.';

-- =============================================================================
-- TABLE: exam_packages
-- =============================================================================

CREATE TABLE exam_packages (
  id                UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
  title             VARCHAR(255) NOT NULL,
  description       TEXT        NOT NULL DEFAULT '',
  duration_minutes  SMALLINT    NOT NULL CHECK (duration_minutes BETWEEN 5 AND 600),
  status            exam_status NOT NULL DEFAULT 'draft',
  created_by        UUID        NOT NULL REFERENCES users (id) ON DELETE RESTRICT,
  created_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  -- Kolom denormalisasi untuk performa baca (diperbarui via trigger)
  question_count    SMALLINT    NOT NULL DEFAULT 0 CHECK (question_count >= 0),
  participant_count INTEGER     NOT NULL DEFAULT 0 CHECK (participant_count >= 0)
);

CREATE INDEX idx_exam_packages_status     ON exam_packages (status);
CREATE INDEX idx_exam_packages_created_by ON exam_packages (created_by);

COMMENT ON TABLE  exam_packages                  IS 'Paket ujian yang dibuat admin.';
COMMENT ON COLUMN exam_packages.question_count   IS 'Cache jumlah soal, diperbarui otomatis via trigger.';
COMMENT ON COLUMN exam_packages.participant_count IS 'Cache jumlah attempt selesai, diperbarui otomatis via trigger.';

-- =============================================================================
-- TABLE: questions
-- =============================================================================

CREATE TABLE questions (
  id               UUID          PRIMARY KEY DEFAULT gen_random_uuid(),
  exam_package_id  UUID          NOT NULL REFERENCES exam_packages (id) ON DELETE CASCADE,
  question_text    TEXT          NOT NULL,
  type             question_type NOT NULL DEFAULT 'multiple_choice',
  order_index      SMALLINT      NOT NULL CHECK (order_index > 0),
  created_at       TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
  updated_at       TIMESTAMPTZ   NOT NULL DEFAULT NOW(),

  CONSTRAINT questions_unique_order UNIQUE (exam_package_id, order_index)
);

CREATE INDEX idx_questions_exam_package ON questions (exam_package_id, order_index);

COMMENT ON TABLE  questions             IS 'Soal-soal dalam setiap paket ujian.';
COMMENT ON COLUMN questions.order_index IS 'Urutan tampil soal, unik per paket ujian.';

-- =============================================================================
-- TABLE: question_options
-- =============================================================================

CREATE TABLE question_options (
  id            UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
  question_id   UUID        NOT NULL REFERENCES questions (id) ON DELETE CASCADE,
  option_text   TEXT        NOT NULL,
  is_correct    BOOLEAN     NOT NULL DEFAULT FALSE,
  label         CHAR(1)     NOT NULL CHECK (label IN ('A','B','C','D','E')),
  created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  CONSTRAINT question_options_unique_label UNIQUE (question_id, label)
);

CREATE INDEX idx_question_options_question ON question_options (question_id);

-- Pastikan tepat satu opsi benar per soal
-- (constraint ini enforced di application level; untuk DB bisa pakai partial unique index)
CREATE UNIQUE INDEX idx_question_options_one_correct
  ON question_options (question_id)
  WHERE is_correct = TRUE;

COMMENT ON TABLE  question_options          IS 'Pilihan jawaban untuk setiap soal.';
COMMENT ON COLUMN question_options.label    IS 'Label opsi: A, B, C, D, atau E.';
COMMENT ON COLUMN question_options.is_correct IS 'Hanya satu opsi per soal yang bernilai TRUE (dijamin via partial unique index).';

-- =============================================================================
-- TABLE: exam_attempts
-- =============================================================================

CREATE TABLE exam_attempts (
  id               UUID           PRIMARY KEY DEFAULT gen_random_uuid(),
  student_id       UUID           NOT NULL REFERENCES users (id) ON DELETE RESTRICT,
  exam_package_id  UUID           NOT NULL REFERENCES exam_packages (id) ON DELETE RESTRICT,
  started_at       TIMESTAMPTZ    NOT NULL DEFAULT NOW(),
  submitted_at     TIMESTAMPTZ,
  status           attempt_status NOT NULL DEFAULT 'in_progress',
  score            SMALLINT       CHECK (score IS NULL OR score BETWEEN 0 AND 100),
  total_score      SMALLINT       NOT NULL DEFAULT 100,
  submit_type      submit_type,
  strike_count     SMALLINT       NOT NULL DEFAULT 0 CHECK (strike_count >= 0),
  created_at       TIMESTAMPTZ    NOT NULL DEFAULT NOW(),
  updated_at       TIMESTAMPTZ    NOT NULL DEFAULT NOW(),

  -- Satu siswa hanya boleh punya satu attempt per paket ujian
  CONSTRAINT exam_attempts_unique_per_student UNIQUE (student_id, exam_package_id)
);

CREATE INDEX idx_exam_attempts_student     ON exam_attempts (student_id);
CREATE INDEX idx_exam_attempts_exam        ON exam_attempts (exam_package_id);
CREATE INDEX idx_exam_attempts_status      ON exam_attempts (status);
CREATE INDEX idx_exam_attempts_submitted   ON exam_attempts (submitted_at DESC) WHERE submitted_at IS NOT NULL;

COMMENT ON TABLE  exam_attempts            IS 'Sesi pengerjaan ujian oleh siswa.';
COMMENT ON COLUMN exam_attempts.score      IS 'Skor 0–100. NULL selama in_progress.';
COMMENT ON COLUMN exam_attempts.strike_count IS 'Akumulasi pelanggaran anti-cheat. Force submit jika > 3.';

-- =============================================================================
-- TABLE: student_answers
-- =============================================================================

CREATE TABLE student_answers (
  id                 UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
  attempt_id         UUID        NOT NULL REFERENCES exam_attempts (id) ON DELETE CASCADE,
  question_id        UUID        NOT NULL REFERENCES questions (id) ON DELETE RESTRICT,
  selected_option_id UUID        REFERENCES question_options (id) ON DELETE RESTRICT,
  is_correct         BOOLEAN,    -- NULL jika belum dijawab
  score              SMALLINT    NOT NULL DEFAULT 0 CHECK (score >= 0),
  answered_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  -- Satu jawaban per soal per attempt
  CONSTRAINT student_answers_unique UNIQUE (attempt_id, question_id)
);

CREATE INDEX idx_student_answers_attempt  ON student_answers (attempt_id);
CREATE INDEX idx_student_answers_question ON student_answers (question_id);

COMMENT ON TABLE  student_answers                  IS 'Jawaban yang dipilih siswa per soal.';
COMMENT ON COLUMN student_answers.selected_option_id IS 'NULL jika soal tidak dijawab.';
COMMENT ON COLUMN student_answers.is_correct       IS 'Dikalkulasi saat submit: bandingkan selected_option_id dengan is_correct di question_options.';

-- =============================================================================
-- TABLE: proctoring_logs
-- =============================================================================

CREATE TABLE proctoring_logs (
  id             UUID           PRIMARY KEY DEFAULT gen_random_uuid(),
  attempt_id     UUID           NOT NULL REFERENCES exam_attempts (id) ON DELETE CASCADE,
  violation_type violation_type NOT NULL,
  occurred_at    TIMESTAMPTZ    NOT NULL DEFAULT NOW(),
  strike_number  SMALLINT       NOT NULL CHECK (strike_number BETWEEN 1 AND 10)
);

CREATE INDEX idx_proctoring_logs_attempt ON proctoring_logs (attempt_id, occurred_at);

COMMENT ON TABLE  proctoring_logs              IS 'Log setiap kejadian pelanggaran anti-cheat.';
COMMENT ON COLUMN proctoring_logs.strike_number IS 'Urutan strike pada attempt ini saat pelanggaran terjadi.';

-- =============================================================================
-- TABLE: app_settings
-- Hanya satu baris (singleton). Gunakan id = 1 sebagai konvensi.
-- =============================================================================

CREATE TABLE app_settings (
  id                  SMALLINT    PRIMARY KEY DEFAULT 1,
  maintenance_mode    BOOLEAN     NOT NULL DEFAULT FALSE,
  maintenance_message TEXT        NOT NULL DEFAULT 'Platform sedang dalam pemeliharaan. Silakan kembali beberapa saat lagi.',
  updated_by          UUID        REFERENCES users (id) ON DELETE SET NULL,
  updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),

  CONSTRAINT app_settings_singleton CHECK (id = 1)
);

COMMENT ON TABLE  app_settings                  IS 'Konfigurasi global aplikasi. Selalu satu baris dengan id = 1.';
COMMENT ON COLUMN app_settings.maintenance_mode IS 'TRUE = halaman siswa menampilkan maintenance page.';

-- =============================================================================
-- TABLE: activity_logs
-- Feed aktivitas yang tampil di Admin Dashboard.
-- =============================================================================

CREATE TABLE activity_logs (
  id           UUID          PRIMARY KEY DEFAULT gen_random_uuid(),
  type         activity_type NOT NULL DEFAULT 'info',
  message      TEXT          NOT NULL,
  occurred_at  TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
  related_user UUID          REFERENCES users (id) ON DELETE SET NULL,
  related_exam UUID          REFERENCES exam_packages (id) ON DELETE SET NULL
);

CREATE INDEX idx_activity_logs_occurred ON activity_logs (occurred_at DESC);

COMMENT ON TABLE  activity_logs IS 'Feed aktivitas untuk dashboard admin. Di-insert oleh backend setelah event penting.';

-- =============================================================================
-- TRIGGERS: auto-update updated_at
-- =============================================================================

CREATE OR REPLACE FUNCTION fn_set_updated_at()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$;

CREATE TRIGGER trg_users_updated_at
  BEFORE UPDATE ON users
  FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

CREATE TRIGGER trg_exam_packages_updated_at
  BEFORE UPDATE ON exam_packages
  FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

CREATE TRIGGER trg_questions_updated_at
  BEFORE UPDATE ON questions
  FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

CREATE TRIGGER trg_exam_attempts_updated_at
  BEFORE UPDATE ON exam_attempts
  FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- =============================================================================
-- TRIGGERS: sync question_count di exam_packages
-- =============================================================================

CREATE OR REPLACE FUNCTION fn_sync_question_count()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
DECLARE
  v_exam_id UUID;
BEGIN
  -- Tentukan exam_package_id dari operasi yang terjadi
  IF TG_OP = 'DELETE' THEN
    v_exam_id := OLD.exam_package_id;
  ELSE
    v_exam_id := NEW.exam_package_id;
  END IF;

  UPDATE exam_packages
  SET    question_count = (
           SELECT COUNT(*) FROM questions WHERE exam_package_id = v_exam_id
         ),
         updated_at = NOW()
  WHERE  id = v_exam_id;

  RETURN NULL; -- AFTER trigger tidak perlu return row
END;
$$;

CREATE TRIGGER trg_question_count_sync
  AFTER INSERT OR DELETE ON questions
  FOR EACH ROW EXECUTE FUNCTION fn_sync_question_count();

-- =============================================================================
-- TRIGGERS: sync participant_count di exam_packages
-- =============================================================================

CREATE OR REPLACE FUNCTION fn_sync_participant_count()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
DECLARE
  v_exam_id UUID;
BEGIN
  IF TG_OP = 'DELETE' THEN
    v_exam_id := OLD.exam_package_id;
  ELSE
    v_exam_id := NEW.exam_package_id;
  END IF;

  UPDATE exam_packages
  SET    participant_count = (
           SELECT COUNT(*)
           FROM   exam_attempts
           WHERE  exam_package_id = v_exam_id
             AND  status IN ('submitted', 'force_submitted', 'time_expired')
         ),
         updated_at = NOW()
  WHERE  id = v_exam_id;

  RETURN NULL;
END;
$$;

CREATE TRIGGER trg_participant_count_sync
  AFTER INSERT OR UPDATE OF status OR DELETE ON exam_attempts
  FOR EACH ROW EXECUTE FUNCTION fn_sync_participant_count();

-- =============================================================================
-- VIEWS
-- =============================================================================

-- View: ringkasan hasil ujian siswa (dipakai halaman riwayat + rekap admin)
CREATE VIEW v_student_results AS
SELECT
  ea.id                                           AS attempt_id,
  ea.student_id,
  u.name                                          AS student_name,
  u.class                                         AS student_class,
  ea.exam_package_id,
  ep.title                                        AS exam_title,
  ea.submitted_at                                 AS completed_at,
  ea.score,
  ea.total_score,
  ea.strike_count,
  ea.submit_type,
  CASE WHEN ea.score >= 75 THEN 'passed' ELSE 'remedial' END AS result_status
FROM  exam_attempts  ea
JOIN  users          u  ON u.id  = ea.student_id
JOIN  exam_packages  ep ON ep.id = ea.exam_package_id
WHERE ea.status IN ('submitted', 'force_submitted', 'time_expired')
  AND ea.score IS NOT NULL;

COMMENT ON VIEW v_student_results IS 'Hasil ujian siswa yang sudah selesai. Threshold lulus = 75.';

-- View: detail jawaban per attempt (dipakai rekap admin accordion)
CREATE VIEW v_attempt_detail AS
SELECT
  sa.attempt_id,
  q.order_index,
  q.question_text,
  sa.question_id,
  sa.selected_option_id,
  sel_opt.option_text                             AS student_answer_text,
  sel_opt.label                                   AS student_option_label,
  cor_opt.option_text                             AS correct_answer_text,
  cor_opt.label                                   AS correct_option_label,
  sa.is_correct,
  sa.score
FROM      student_answers  sa
JOIN      questions         q        ON q.id  = sa.question_id
LEFT JOIN question_options  sel_opt  ON sel_opt.id = sa.selected_option_id
JOIN      question_options  cor_opt
            ON cor_opt.question_id = sa.question_id
           AND cor_opt.is_correct  = TRUE
ORDER BY  sa.attempt_id, q.order_index;

COMMENT ON VIEW v_attempt_detail IS 'Detail jawaban benar/salah per soal per attempt.';

-- View: dashboard stats untuk admin (semua kalkulasi real-time)
CREATE VIEW v_dashboard_stats AS
SELECT
  (SELECT COUNT(*) FROM users WHERE role = 'student' AND status = 'active')           AS total_students,
  (SELECT COUNT(*) FROM exam_packages)                                                  AS total_exam_packages,
  (SELECT COUNT(*) FROM exam_packages WHERE status = 'published')                       AS active_exams,
  (SELECT COUNT(*) FROM exam_attempts WHERE started_at::date = CURRENT_DATE)            AS today_attempts,
  (SELECT COUNT(*) FROM exam_attempts WHERE status IN ('submitted','force_submitted','time_expired')) AS total_completed_attempts;

COMMENT ON VIEW v_dashboard_stats IS 'Statistik ringkasan untuk admin dashboard.';

-- =============================================================================
-- FUNCTIONS (helper untuk .NET backend)
-- =============================================================================

-- Kalkulasi & simpan skor saat submit ujian
CREATE OR REPLACE FUNCTION fn_submit_exam(
  p_attempt_id  UUID,
  p_submit_type submit_type
)
RETURNS TABLE (
  score        SMALLINT,
  total_score  SMALLINT,
  attempt_status attempt_status
)
LANGUAGE plpgsql AS $$
DECLARE
  v_exam_id      UUID;
  v_total_q      INTEGER;
  v_correct_count INTEGER;
  v_score        SMALLINT;
  v_status       attempt_status;
BEGIN
  -- Ambil exam_package_id
  SELECT exam_package_id INTO v_exam_id
  FROM   exam_attempts WHERE id = p_attempt_id AND status = 'in_progress';

  IF NOT FOUND THEN
    RAISE EXCEPTION 'Attempt % tidak ditemukan atau sudah selesai.', p_attempt_id;
  END IF;

  -- Hitung total soal dalam paket
  SELECT COUNT(*) INTO v_total_q
  FROM   questions WHERE exam_package_id = v_exam_id;

  -- Tandai is_correct di student_answers dan hitung yang benar
  UPDATE student_answers sa
  SET    is_correct = (
           sa.selected_option_id IS NOT NULL
           AND EXISTS (
             SELECT 1 FROM question_options qo
             WHERE qo.id = sa.selected_option_id AND qo.is_correct = TRUE
           )
         ),
         score = CASE
           WHEN sa.selected_option_id IS NOT NULL
                AND EXISTS (
                  SELECT 1 FROM question_options qo
                  WHERE qo.id = sa.selected_option_id AND qo.is_correct = TRUE
                )
           THEN (100 / v_total_q)
           ELSE 0
         END
  WHERE sa.attempt_id = p_attempt_id;

  SELECT COALESCE(SUM(score), 0) INTO v_correct_count
  FROM   student_answers WHERE attempt_id = p_attempt_id;

  v_score := LEAST(v_correct_count::SMALLINT, 100);

  -- Tentukan status akhir
  v_status := CASE p_submit_type
    WHEN 'force_anticheat'   THEN 'force_submitted'::attempt_status
    WHEN 'auto_time_expired' THEN 'time_expired'::attempt_status
    ELSE                          'submitted'::attempt_status
  END;

  -- Update attempt
  UPDATE exam_attempts
  SET    status       = v_status,
         score        = v_score,
         submit_type  = p_submit_type,
         submitted_at = NOW(),
         updated_at   = NOW()
  WHERE  id = p_attempt_id;

  RETURN QUERY
  SELECT v_score, 100::SMALLINT, v_status;
END;
$$;

COMMENT ON FUNCTION fn_submit_exam IS 'Kalkulasi skor, update is_correct per jawaban, dan ubah status attempt menjadi selesai.';

-- Laporan pelanggaran anti-cheat
CREATE OR REPLACE FUNCTION fn_report_violation(
  p_attempt_id    UUID,
  p_violation_type violation_type
)
RETURNS TABLE (
  strike_count  SMALLINT,
  force_submit  BOOLEAN
)
LANGUAGE plpgsql AS $$
DECLARE
  v_new_strike SMALLINT;
BEGIN
  -- Increment strike_count di attempt
  UPDATE exam_attempts
  SET    strike_count = strike_count + 1,
         updated_at   = NOW()
  WHERE  id = p_attempt_id AND status = 'in_progress'
  RETURNING strike_count INTO v_new_strike;

  IF NOT FOUND THEN
    RAISE EXCEPTION 'Attempt % tidak ditemukan atau sudah selesai.', p_attempt_id;
  END IF;

  -- Insert log
  INSERT INTO proctoring_logs (attempt_id, violation_type, strike_number)
  VALUES (p_attempt_id, p_violation_type, v_new_strike);

  RETURN QUERY
  SELECT v_new_strike, (v_new_strike > 3);
END;
$$;

COMMENT ON FUNCTION fn_report_violation IS 'Tambah strike, insert proctoring_log, dan kembalikan apakah harus force submit.';

-- =============================================================================
-- ROW LEVEL SECURITY (opsional, aktifkan jika pakai PostgREST / Supabase)
-- =============================================================================
-- ALTER TABLE users           ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE exam_attempts   ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE student_answers ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE proctoring_logs ENABLE ROW LEVEL SECURITY;
--
-- Contoh policy: siswa hanya bisa lihat data milik sendiri
-- CREATE POLICY student_own_attempts ON exam_attempts
--   FOR ALL TO authenticated
--   USING (student_id = current_setting('app.current_user_id')::UUID);

-- =============================================================================
-- SELESAI: schema.sql
-- Lanjutkan dengan menjalankan seed.sql
-- =============================================================================
