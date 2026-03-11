namespace School.Domain.Constants;

public static class ValidationMessages
{
    public const string StudentNameRequired = "O nome do aluno é obrigatório.";
    public const string StudentNameTooLong = "O nome deve ter no máximo 50 caracteres.";
    public const string InvalidEmailDomain = "O e-mail deve pertencer ao domínio @faculdade.edu.";
    public const string DuplicateEmail = "Já existe um aluno matriculado com este e-mail.";
    public const string StudentNotFound = "Aluno não encontrado.";
    public const string CourseNotFound = "Curso não encontrado.";
    public const string CourseTitleRequired = "O título do curso é obrigatório.";
    public const string InvalidWorkload = "A carga horária é inválida.";
    public const string EnrollmentLimitExceeded = "Limite de carga horária excedido.";
    public const string EnrollmentDuplicate = "Aluno já está matriculado neste curso.";
    public const string EnrollmentNotFound = "Matrícula não encontrada.";
    public const string InvalidStudentId = "ID do aluno inválido.";
    public const string InvalidCourseId = "ID do curso inválido.";
}
