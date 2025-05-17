# API de Gestión de Citas Médicas

Esta es una API REST desarrollada en .NET C# para gestionar un sistema de citas médicas. Permite manejar la información de pacientes, médicos, citas y otros datos relacionados, todo conectado a una base de datos MySQL.

## Tecnologías utilizadas

- .NET 6 / .NET Core  
- C#  
- MySQL  
- Entity Framework Core
- JWT para autenticación y autorización  

## Instalación

1. Clona este repositorio:  
   ```bash
   git clone https://github.com/tuusuario/tu-repo.git
2. Entra al directorio del proyecto:
   ```bash
   cd tu-repo

4. Configura la cadena de conexión a MySQL en el archivo appsettings.json o donde tengas configurada la conexión:
   ```bash
   "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=tu_basedatos;user=usuario;password=contraseña;"
   }

5. Restaura las dependencias y compila el proyecto:
   ```bash
   dotnet restore
   dotnet build

6. Ejecuta la API:
   ```bash
   dotnet run

## Uso:

### La API está protegida mediante JWT. Para usarla correctamente, sigue estos pasos:

#### 1. Registro:
Primero, debes autenticarte enviando tus credenciales para obtener un token JWT.  
##### Ejemplo usando curl:  
```bash
curl -X POST https://tuapi.com/Api/Registro \
  -H "Content-Type: application/json" \
  -d {
        "name":  "tu nombre completo",
        "email": "tucorreo@gmail.com",
        "pass":  "tu contraseña",
        "tipo":  "tipo de usuario("Paciente" o "Medico")"
      }
```
##### Respuesta exitosa (200 OK):
```bash
{
    "mensaje": "Registro enviado correctamente",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cC..."
}
```
##### Notas:  

- El `token` devuelto debe guardarse y usarse en el encabezado Authorization en sguientes solicitudes protegidas.
- Es importante que el email sea válido, ya que puede ser utilizado posteriormente para verificación o recuperación de cuenta.

#### 2. Envío de verificación de correo electrónico:  
Para garantizar la seguridad y validez de los usuarios registrados, la API incluye un proceso de verificación de correo electrónico.
##### Ejemplo usando curl;
```bash
curl -X POST https://tuapi.com/Api/Registro/Enviar-codigo \
  -H "Content-Type: application/json" \
  -d {
        "token": "eyJhbGciOiJIUzI1NiIsInR5c..."
     }
```
##### Respuesta exitosa (200 OK): 
```bash
{
    "mensaje": "Código enviado correctamente",
    "tokenCodigo": "eyJhbGciOiJIUzI1NiIsInR5c..."
}
```
##### Notas:

- El `tokenCodigo` es un nuevo token temporal asociado al código enviado, que se utilizará en el siguiente paso para verificar el código ingresado por el usuario.  
- Asegúrate de que el correo electrónico sea accesible y válido, ya que el código de verificación se enviará allí.
- Este paso es esencial para habilitar completamente la cuenta del usuario.

#### 3. Confirmación del código de verificación:  
Después de recibir el código en tu correo, deberás enviarlo junto con el `tokenCodigo` para completar el proceso de verificación.
##### Ejemplo usando curl;
```bash
curl -X POST https://tuapi.com/Api/Registro/Confirmar-codigo \
  -H "Content-Type: application/json" \
  -d {
        "tokenCodigo": "eyJhbGciOiJIUzI1NiIsInR5cCI6I9...",
        "codigo": (Tiene que ser de tipo num)
     }
```
##### Respuesta exitosa (200 OK): 
```bash
{
    "mensaje": "Código confirmado correctamente"
}
```
##### Notas:

- El código tiene un tiempo limitado de validez (normalmente de 60 minutos).
- Si el código expira, el usuario deberá solicitar uno nuevo utilizando el endpoint `/Api/Registro/Enviar-codigo`.
- Este sistema ayuda a prevenir cuentas falsas o maliciosas, mejorando la seguridad general del sistema.

### Endpoints principales

- `POST /Api/Registro` - Registro de usuarios
- `POST /Api/Registro/Enviar-codigo` - Enviar código de verificación
- `POST /Api/Registro/Confirmar-codigo` - Confirmar código
- `POST /Api/Login` - Inicio de sesión
  
