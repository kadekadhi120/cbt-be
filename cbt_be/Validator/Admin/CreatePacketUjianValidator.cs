using cbt.be.Models.RequestModels.Admin.ManagementUjian;
using FluentValidation;

namespace cbt.be.Validator.Admin
{
    public class CreatePacketUjianValidator : AbstractValidator<CreatePacketUjianRequest>
    {
        public CreatePacketUjianValidator() 
        {
            RuleFor(x => x.Title)
                   .NotEmpty().WithMessage("Judul ujian tidak boleh kosong.")
                   .MaximumLength(150).WithMessage("Judul ujian maksimal 150 karakter.");

            RuleFor(x => x.Description)
                   .NotEmpty().WithMessage("Deskripsi ujian tidak boleh kosong.")
                   .MaximumLength(500).WithMessage("Deskripsi maksimal 500 karakter.");

            RuleFor(x => x.DurationMinutes)
                   .GreaterThan(0).WithMessage("Durasi waktu ujian harus lebih dari 0 menit.");

            RuleFor(x => x.ClassCode)
                   .NotNull().WithMessage("Daftar kelas tidak boleh null.")
                   .NotEmpty().WithMessage("Paket ujian harus dipilih minimal untuk 1 kelas.");

            RuleForEach(x => x.ClassCode)
                   .NotEmpty().WithMessage("Kode kelas tidak boleh ada yang kosong.");
        }
    }
}
