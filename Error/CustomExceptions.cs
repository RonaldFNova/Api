namespace API.Error
{
    public class UsuarioNoEncontradoException : Exception
    {
        public UsuarioNoEncontradoException() 
            : base("El usuario no fue encontrado.") {}
    }

    public class CodigoIncorrectoException : Exception
    {
        public CodigoIncorrectoException()
            : base("El código ingresado es incorrecto.") {}
    }

    public class EmailNoEncontradoException : Exception
    {
        public EmailNoEncontradoException()
            : base("El email no fue encontrado") {}
    }

    public class EstadoUsuarioVerificadoException : Exception
    {
        public EstadoUsuarioVerificadoException()
            : base("El usuario ya esta verificado o el id de la cuenta no fue encontrado") {}
    }

        public class EstadoEmailVerificadoException : Exception
    {
        public EstadoEmailVerificadoException()
            : base("El usuario ya esta verificado o el email de la cuenta no fue encontrado") {}
    }


}
