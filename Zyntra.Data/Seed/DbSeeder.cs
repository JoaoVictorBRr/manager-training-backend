using System.Security.Cryptography;
using Zyntra.Data.Context;
using Zyntra.Domain.Entities;
using Zyntra.Domain.Enum;

namespace Zyntra.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(ZyntraContext context)
    {
        if (context.User.Any()) return;

        var now = DateTime.UtcNow;

        // === USERS ===
        var (adminHash, adminSalt) = Hash("Admin@123");
        var (instHash, instSalt) = Hash("Instrutor@123");
        var (alunoHash, alunoSalt) = Hash("Aluno@123");

        var adminUser = new User { Name = "Admin Zyntra", CellphoneNumber = "11999990000", Cpf = "00000000001", Email = "admin@zyntra.com", Password = adminHash, Salt = adminSalt, Role = Role.Administrator, Token = string.Empty, UserIdCreated = 0 };
        var carlosUser = new User { Name = "Carlos Souza", CellphoneNumber = "11999990001", Cpf = "00000000002", Email = "carlos@zyntra.com", Password = instHash, Salt = instSalt, Role = Role.Instructor, Token = string.Empty, UserIdCreated = 0 };
        var anaUser = new User { Name = "Ana Lima", CellphoneNumber = "11999990002", Cpf = "00000000003", Email = "ana@zyntra.com", Password = instHash, Salt = instSalt, Role = Role.Instructor, Token = string.Empty, UserIdCreated = 0 };
        var mariaUser = new User { Name = "Maria Oliveira", CellphoneNumber = "11999990003", Cpf = "00000000004", Email = "maria@zyntra.com", Password = alunoHash, Salt = alunoSalt, Role = Role.Student, Token = string.Empty, UserIdCreated = 0 };
        var pedroUser = new User { Name = "Pedro Santos", CellphoneNumber = "11999990004", Cpf = "00000000005", Email = "pedro@zyntra.com", Password = alunoHash, Salt = alunoSalt, Role = Role.Student, Token = string.Empty, UserIdCreated = 0 };
        var julianaUser = new User { Name = "Juliana Costa", CellphoneNumber = "11999990005", Cpf = "00000000006", Email = "juliana@zyntra.com", Password = alunoHash, Salt = alunoSalt, Role = Role.Student, Token = string.Empty, UserIdCreated = 0 };

        context.User.AddRange(adminUser, carlosUser, anaUser, mariaUser, pedroUser, julianaUser);
        await context.SaveChangesAsync();

        // === ADMINISTRATOR, INSTRUCTORS, STUDENTS ===
        var admin = new Administrator { UserId = adminUser.Id, AccessLevel = 1, UserIdCreated = 0 };
        context.Administrator.Add(admin);

        var carlos = new Instructor { UserId = carlosUser.Id, Specialty = "Musculação, Funcional", Cref = "001234-G/SP", UserIdCreated = 0 };
        var ana = new Instructor { UserId = anaUser.Id, Specialty = "Yoga, Pilates", Cref = "005678-G/SP", UserIdCreated = 0 };
        context.Instructor.AddRange(carlos, ana);

        var maria = new Student { UserId = mariaUser.Id, BirthDate = new DateTime(1995, 3, 15), PaymentStatus = "Paid", UserIdCreated = 0 };
        var pedro = new Student { UserId = pedroUser.Id, BirthDate = new DateTime(1990, 7, 22), PaymentStatus = "Pending", UserIdCreated = 0 };
        var juliana = new Student { UserId = julianaUser.Id, BirthDate = new DateTime(1998, 11, 10), PaymentStatus = "Overdue", UserIdCreated = 0 };
        context.Student.AddRange(maria, pedro, juliana);
        await context.SaveChangesAsync();

        // === CLASSES ===
        var muscClass = new Class { Modality = "Musculação", DateTime = now.AddDays(1).Date.AddHours(18), MaxCapacity = 20, AvailableSlots = 15, Unit = "Unidade Centro", InstructorId = carlos.Id, UserIdCreated = 0 };
        var yogaClass = new Class { Modality = "Yoga", DateTime = now.AddDays(2).Date.AddHours(7), MaxCapacity = 15, AvailableSlots = 10, Unit = "Unidade Centro", InstructorId = ana.Id, UserIdCreated = 0 };
        var funcClass = new Class { Modality = "Funcional", DateTime = now.AddDays(3).Date.AddHours(19), MaxCapacity = 20, AvailableSlots = 18, Unit = "Unidade Centro", InstructorId = carlos.Id, UserIdCreated = 0 };
        var pilatesClass = new Class { Modality = "Pilates", DateTime = now.AddDays(4).Date.AddHours(10), MaxCapacity = 12, AvailableSlots = 8, Unit = "Unidade Centro", InstructorId = ana.Id, UserIdCreated = 0 };
        var spinClass = new Class { Modality = "Spinning", DateTime = now.AddDays(5).Date.AddHours(6).AddMinutes(30), MaxCapacity = 15, AvailableSlots = 12, Unit = "Unidade Centro", InstructorId = carlos.Id, UserIdCreated = 0 };
        context.Class.AddRange(muscClass, yogaClass, funcClass, pilatesClass, spinClass);
        await context.SaveChangesAsync();

        // === WORKOUT SHEETS ===
        var mariaSheet = new WorkoutSheet { StudentId = maria.Id, InstructorId = carlos.Id, StartDate = now.AddMonths(-1), IsActive = true, Notes = "Treino A - Hipertrofia", UserIdCreated = 0 };
        var julianaSheet = new WorkoutSheet { StudentId = juliana.Id, InstructorId = ana.Id, StartDate = now.AddMonths(-1), IsActive = true, Notes = "Treino B - Emagrecimento", UserIdCreated = 0 };
        context.WorkoutSheet.AddRange(mariaSheet, julianaSheet);
        await context.SaveChangesAsync();

        // === EXERCISES ===
        context.Exercise.AddRange(
            new Exercise { WorkoutSheetId = mariaSheet.Id, Name = "Supino Reto", MuscleGroup = "Peitoral", Sets = 4, Repetitions = "12", SuggestedLoad = 60, VideoUrl = null, Description = "Supino reto com barra livre.", RestTime = "90s", UserIdCreated = 0 },
            new Exercise { WorkoutSheetId = mariaSheet.Id, Name = "Agachamento Livre", MuscleGroup = "Quadríceps", Sets = 4, Repetitions = "10-12", SuggestedLoad = 80, VideoUrl = null, Description = "Agachamento com barra.", RestTime = "120s", UserIdCreated = 0 },
            new Exercise { WorkoutSheetId = mariaSheet.Id, Name = "Puxada na Frente", MuscleGroup = "Costas", Sets = 3, Repetitions = "12", SuggestedLoad = 50, VideoUrl = null, Description = "Puxada frontal na polia alta.", RestTime = "90s", UserIdCreated = 0 },
            new Exercise { WorkoutSheetId = mariaSheet.Id, Name = "Desenvolvimento", MuscleGroup = "Ombros", Sets = 3, Repetitions = "12", SuggestedLoad = 40, VideoUrl = null, Description = "Desenvolvimento com halteres.", RestTime = "75s", UserIdCreated = 0 },
            new Exercise { WorkoutSheetId = julianaSheet.Id, Name = "Cadeira Extensora", MuscleGroup = "Quadríceps", Sets = 3, Repetitions = "20", SuggestedLoad = 40, VideoUrl = null, Description = "Extensão de pernas na cadeira extensora.", ToFailure = true, UserIdCreated = 0 },
            new Exercise { WorkoutSheetId = julianaSheet.Id, Name = "Esteira Inclinada", MuscleGroup = "Cardio", Sets = 3, Repetitions = "20min", SuggestedLoad = 0, VideoUrl = null, Description = "20 minutos em esteira com inclinação moderada.", UserIdCreated = 0 },
            new Exercise { WorkoutSheetId = julianaSheet.Id, Name = "Abdominal Bicicleta", MuscleGroup = "Core", Sets = 3, Repetitions = "20", SuggestedLoad = 0, VideoUrl = null, Description = "Abdominal bicicleta alternado.", UserIdCreated = 0 }
        );

        // === PHYSICAL ASSESSMENTS ===
        context.PhysicalAssessment.AddRange(
            new PhysicalAssessment { StudentId = maria.Id, AssessmentDate = now.AddMonths(-1), Weight = 65m, Height = 1.65m, Bmi = 23.88m, BodyFatPercentage = 25m, Measurements = null, Notes = "Avaliação inicial.", UserIdCreated = 0 },
            new PhysicalAssessment { StudentId = juliana.Id, AssessmentDate = now.AddMonths(-1), Weight = 72m, Height = 1.70m, Bmi = 24.91m, BodyFatPercentage = 30m, Measurements = null, Notes = "Avaliação inicial.", UserIdCreated = 0 }
        );

        // === SCHEDULES ===
        context.Schedule.AddRange(
            new Schedule { StudentId = maria.Id, ClassId = muscClass.Id, Status = ScheduleStatus.Confirmed, ReservationDate = now.AddDays(-2), UserIdCreated = 0 },
            new Schedule { StudentId = maria.Id, ClassId = yogaClass.Id, Status = ScheduleStatus.Confirmed, ReservationDate = now.AddDays(-1), UserIdCreated = 0 },
            new Schedule { StudentId = pedro.Id, ClassId = funcClass.Id, Status = ScheduleStatus.Confirmed, ReservationDate = now.AddDays(-1), UserIdCreated = 0 }
        );

        // === PAYMENTS ===
        context.Payment.AddRange(
            new Payment { StudentId = maria.Id, Amount = 149.90m, DueDate = now.AddDays(5), PaymentDate = now.AddDays(-25), PaymentStatus = PaymentStatus.Paid, PaymentMethod = "Pix", UserIdCreated = 0 },
            new Payment { StudentId = maria.Id, Amount = 149.90m, DueDate = now.AddDays(-25), PaymentDate = now.AddDays(-26), PaymentStatus = PaymentStatus.Paid, PaymentMethod = "Pix", UserIdCreated = 0 },
            new Payment { StudentId = pedro.Id, Amount = 149.90m, DueDate = now.AddDays(3), PaymentDate = null, PaymentStatus = PaymentStatus.Pending, PaymentMethod = null, UserIdCreated = 0 },
            new Payment { StudentId = juliana.Id, Amount = 149.90m, DueDate = now.AddDays(-5), PaymentDate = null, PaymentStatus = PaymentStatus.Overdue, PaymentMethod = null, UserIdCreated = 0 },
            new Payment { StudentId = juliana.Id, Amount = 149.90m, DueDate = now.AddDays(-35), PaymentDate = null, PaymentStatus = PaymentStatus.Overdue, PaymentMethod = null, UserIdCreated = 0 }
        );

        // === CHECK-INS ===
        context.CheckIn.AddRange(
            new CheckIn { StudentId = maria.Id, DateTime = now.AddDays(-1).Date.AddHours(18).AddMinutes(30), Unit = "Unidade Centro", AccessType = CheckInType.App, ValidationStatus = CheckInStatus.Approved, UserIdCreated = 0 },
            new CheckIn { StudentId = maria.Id, DateTime = now.AddDays(-3).Date.AddHours(19), Unit = "Unidade Centro", AccessType = CheckInType.App, ValidationStatus = CheckInStatus.Approved, UserIdCreated = 0 },
            new CheckIn { StudentId = juliana.Id, DateTime = now.AddDays(-2).Date.AddHours(7).AddMinutes(15), Unit = "Unidade Centro", AccessType = CheckInType.App, ValidationStatus = CheckInStatus.Approved, UserIdCreated = 0 }
        );

        // === CHAT MESSAGES ===
        context.ChatMessage.AddRange(
            new ChatMessage { StudentId = maria.Id, InstructorId = carlos.Id, Message = "Olá professor, posso alterar algum exercício do treino?", MessageDateTime = now.AddDays(-1).AddHours(-2), IsRead = true, UserIdCreated = 0 },
            new ChatMessage { StudentId = maria.Id, InstructorId = carlos.Id, Message = "Claro! Me diz qual exercício e vou ajustar sua ficha.", MessageDateTime = now.AddDays(-1).AddHours(-1), IsRead = false, UserIdCreated = 0 }
        );

        // === NOTIFICATIONS ===
        context.Notification.AddRange(
            new Notification { UserId = mariaUser.Id, Type = NotificationType.ClassReminder, Title = "Lembrete de Aula", Message = "Você tem Musculação amanhã às 18h.", SendDateTime = now.AddHours(-2), IsRead = false, UserIdCreated = 0 },
            new Notification { UserId = pedroUser.Id, Type = NotificationType.PaymentDue, Title = "Pagamento Pendente", Message = "Sua mensalidade vence em 3 dias.", SendDateTime = now.AddHours(-3), IsRead = false, UserIdCreated = 0 },
            new Notification { UserId = julianaUser.Id, Type = NotificationType.PaymentDue, Title = "Pagamento Vencido", Message = "Sua mensalidade está vencida há 5 dias. Regularize para continuar treinando.", SendDateTime = now.AddHours(-1), IsRead = false, UserIdCreated = 0 },
            new Notification { UserId = mariaUser.Id, Type = NotificationType.WorkoutReminder, Title = "Hora de Treinar", Message = "Não esqueça do seu treino hoje!", SendDateTime = now.AddHours(-4), IsRead = true, UserIdCreated = 0 }
        );

        // === PARTNER INTEGRATIONS ===
        context.PartnerIntegration.AddRange(
            new PartnerIntegration { PartnerName = "Gympass", IntegrationType = PartnerType.Gympass, Token = "GYMPASS-VALID-TOKEN-001", ValidationStatus = CheckInStatus.Approved, UserIdCreated = 0 },
            new PartnerIntegration { PartnerName = "TotalPass", IntegrationType = PartnerType.TotalPass, Token = "TOTALPASS-VALID-TOKEN-001", ValidationStatus = CheckInStatus.Approved, UserIdCreated = 0 }
        );

        await context.SaveChangesAsync();
    }

    // PBKDF2-SHA256 com 100.000 iterações — mesmo algoritmo do PasswordHasher.HashPassword()
    private static (string hash, string salt) Hash(string password)
    {
        byte[] saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
        byte[] hashBytes = pbkdf2.GetBytes(32);
        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }
}
