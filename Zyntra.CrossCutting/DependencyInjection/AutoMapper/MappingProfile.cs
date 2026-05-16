using AutoMapper;
using Zyntra.Domain.Dtos.ChatDto;
using Zyntra.Domain.Dtos.CheckInDto;
using Zyntra.Domain.Dtos.ClassDto;
using Zyntra.Domain.Dtos.ExerciseDto;
using Zyntra.Domain.Dtos.InstructorDto;
using Zyntra.Domain.Dtos.NotificationDto;
using Zyntra.Domain.Dtos.PaymentDto;
using Zyntra.Domain.Dtos.PhysicalAssessmentDto;
using Zyntra.Domain.Dtos.ScheduleDto;
using Zyntra.Domain.Dtos.StudentDto;
using Zyntra.Domain.Dtos.WorkoutSheetDto;
using Zyntra.Domain.Entities;

namespace Zyntra.CrossCutting.DependencyInjection.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<StudentCreateDto, User>();
        CreateMap<Student, StudentResponseDto>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.User.Name))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
            .ForMember(d => d.Cpf, o => o.MapFrom(s => s.User.Cpf))
            .ForMember(d => d.CellphoneNumber, o => o.MapFrom(s => s.User.CellphoneNumber));

        CreateMap<InstructorCreateDto, User>();
        CreateMap<Instructor, InstructorResponseDto>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.User.Name))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email));

        CreateMap<ClassCreateDto, Class>();
        CreateMap<ClassUpdateDto, Class>();
        CreateMap<Class, ClassResponseDto>()
            .ForMember(d => d.InstructorName, o => o.MapFrom(s => s.Instructor.User.Name));

        CreateMap<Schedule, ScheduleResponseDto>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.User.Name))
            .ForMember(d => d.ClassModality, o => o.MapFrom(s => s.Class.Modality))
            .ForMember(d => d.ClassDateTime, o => o.MapFrom(s => s.Class.DateTime));

        CreateMap<CheckIn, CheckInResponseDto>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.User.Name));

        CreateMap<WorkoutSheet, WorkoutSheetResponseDto>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.User.Name))
            .ForMember(d => d.InstructorName, o => o.MapFrom(s => s.Instructor.User.Name));

        CreateMap<ExerciseCreateDto, Exercise>();
        CreateMap<ExerciseUpdateDto, Exercise>();
        CreateMap<Exercise, ExerciseResponseDto>();

        CreateMap<PhysicalAssessmentCreateDto, PhysicalAssessment>();
        CreateMap<PhysicalAssessment, PhysicalAssessmentResponseDto>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.User.Name));

        CreateMap<Payment, PaymentResponseDto>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.User.Name));

        CreateMap<ChatMessage, ChatMessageResponseDto>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.User.Name))
            .ForMember(d => d.InstructorName, o => o.MapFrom(s => s.Instructor.User.Name));

        CreateMap<Notification, NotificationResponseDto>();
    }
}
