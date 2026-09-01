# SmartCBT — Database PostgreSQL

## Cara Menjalankan

```bash
# 1. Buat database baru
createdb smartcbt

# 2. Jalankan schema (struktur tabel, enum, trigger, view, function)
psql -d smartcbt -f schema.sql

# 3. Isi data seed / demo
psql -d smartcbt -f seed.sql

# Atau sekaligus:
psql -d smartcbt -f schema.sql -f seed.sql
```

## Struktur Tabel

| Tabel | Deskripsi |
|---|---|
| `users` | Admin + siswa (single table, dibedakan kolom `role`) |
| `exam_packages` | Paket ujian yang dibuat admin |
| `questions` | Soal-soal per paket ujian |
| `question_options` | Pilihan jawaban per soal (A–E, satu `is_correct = true`) |
| `exam_attempts` | Sesi pengerjaan ujian oleh siswa |
| `student_answers` | Jawaban yang dipilih siswa per soal |
| `proctoring_logs` | Log pelanggaran anti-cheat |
| `app_settings` | Konfigurasi global (singleton, id = 1) |
| `activity_logs` | Feed aktivitas untuk dashboard admin |

## Views Siap Pakai

| View | Kegunaan |
|---|---|
| `v_student_results` | Hasil ujian siswa + status lulus/remedial |
| `v_attempt_detail` | Detail jawaban benar/salah per soal per attempt |
| `v_dashboard_stats` | Statistik untuk admin dashboard (real-time) |

## Functions

| Function | Kegunaan |
|---|---|
| `fn_submit_exam(attempt_id, submit_type)` | Kalkulasi skor + ubah status attempt |
| `fn_report_violation(attempt_id, violation_type)` | Tambah strike + insert proctoring log |

### Contoh penggunaan:

```sql
-- Submit ujian secara manual
SELECT * FROM fn_submit_exam(
  '40000000-0000-0000-0000-000000000001',
  'manual'
);

-- Lapor pelanggaran tab switch
SELECT * FROM fn_report_violation(
  '40000000-0000-0000-0000-000000000001',
  'tab_switch'
);
-- Returns: strike_count, force_submit (true jika strike > 3)
```

## Akun Demo

| Nama | Email | Password | Role |
|---|---|---|---|
| Admin SmartCBT | admin@smartcbt.id | password123 | admin |
| Budi Santoso | budi@siswa.id | password123 | student |
| Siti Rahayu | siti@siswa.id | password123 | student |
| Ahmad Fauzi | ahmad@siswa.id | password123 | student |
| Dewi Lestari | dewi@siswa.id | password123 | student (nonaktif) |
| Rizky Pratama | rizky@siswa.id | password123 | student |

## Catatan untuk Backend .NET

- Semua PK menggunakan `UUID` — gunakan `Guid` di C#.
- Password disimpan sebagai **bcrypt hash** — gunakan library `BCrypt.Net-Next`.
- Kolom `question_count` dan `participant_count` di `exam_packages` diperbarui otomatis via **trigger PostgreSQL**, tidak perlu diupdate manual dari backend.
- Untuk submit ujian, panggil function `fn_submit_exam()` daripada kalkulasi di aplikasi — ini memastikan skor konsisten meski dipanggil bersamaan.
- Threshold lulus = **75** (bisa dikonfigurasi ulang di view `v_student_results`).
