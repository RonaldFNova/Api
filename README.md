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
##### Ejemplo usando curl:
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
##### Ejemplo usando curl:
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

#### 4. Inicio de sesión de usuario (Login)
Este endpoint permite a un usuario autenticarse en el sistema proporcionando su correo electrónico y contraseña. Si las credenciales son válidas, se devuelve un token JWT que puede usarse en solicitudes protegidas.  
##### Ejemplo usando curl:
```bash
curl -X POST https://tuapi.com/Api/Login \
  -H "Content-Type: application/json" \
  -d '{
        "email": "usuario@correo.com",
        "pass": "tu_contraseña_segura"
      }'
```

##### Respuesta exitosa (200 OK):  
```bash
{
    "mensaje": "Código reenviado correctamente",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user_type": "Doctor"
}
```

##### Notas  
- El token devuelto es un JWT (JSON Web Token) que debes enviar en las solicitudes posteriores para acceder a recursos protegidos.
- El token devuelto representa la sesión del usuario y debe ser almacenado de forma segura en el cliente (por ejemplo, en localStorage o sessionStorage).
- Se recomienda validar que el correo tenga formato válido antes de enviar.


#### 5. Consulta del tipo de usuario (Tipo-user)
Este endpoint permite al sistema identificar el tipo de cuenta asociada al token de autenticación. Es útil para determinar los permisos y roles del usuario (por ejemplo, "paciente", "doctor", etc.).
##### Ejemplo usando curl:

```bash
curl -X POST https://tuapi.com/Api/Tipo-user \
  -H "Content-Type: application/json" \
  -d '{
        "token": "eyJhbGciOiJIUzI1NiIsInR5c..."
      }'
```
##### Respuesta exitosa (200 OK):  
```bash
{
    "mensaje": "Tipo de usuario obtenido correctamente",
    "tipo": "doctor"
}

```
##### Notas:
- El campo "tipo" puede devolver valores como "admin", "paciente", "doctor", etc., según la lógica de tu sistema.  
- El token debe ser válido y corresponder a un usuario autenticado previamente.  
- Este endpoint puede usarse tras el login o verificación para redirigir al usuario a la sección correspondiente de la aplicación según su rol.


#### 6. Inserción de información personal del paciente (Personal-Paciente)
Este endpoint permite registrar o actualizar los datos personales de un paciente una vez que ya ha sido autenticado. Es esencial para completar el perfil del usuario con información clínica y de contacto relevante.
##### Ejemplo usando curl;
```bash
curl -X POST https://tuapi.com/Api/Informacion-Personal-Paciente \
  -H "Content-Type: application/json" \
  -d '{
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpX...,
        "cell": "numero de telefono",
        "tipoId": "Tipo de identificacion (Cedula de ciudadania, Cedula extranjera, otra)",
        "personalId": "Numero de identificacion",
        "bloodGroup": "Grupo sanguineo (A, B, AB, O)",
        "alergiasGeneral": [(lista de las alergias (No son obliagatorios))],
        "alergiasMedications": [(lista de las alergias a medicamentos (No son obliagatorios))]
     }'
```

##### Respuesta exitosa (200 OK):
```bash
{
    "mensaje": "La informacion enviada del paciente se registro correctamente"
}
```

##### Notas:
- El campo token debe ser válido y pertenecer a un usuario con tipo paciente.  
- Los campos como   `cell `,  `bloodGroup `,  `personalId ` y  `tipoId ` son requeridos para tener un perfil completo.  
- Si ya existe información previa, el backend puede optar por actualizarla o retornar un error dependiendo de la lógica implementada.  


#### 7. Inserción de información personal del doctor (Personal-Doctor)  
Este endpoint permite registrar o actualizar los datos personales de un doctor una vez que ya ha sido autenticado. Es crucial para identificar sus credenciales y especialidades médicas dentro del sistema.
##### Ejemplo usando curl:
```bash
curl -X POST https://tuapi.com/Api/Informacion-Personal-Doctor \
  -H "Content-Type: application/json" \
  -d '{
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpX...,
        "cell": "Número de teléfono",
        "tipoId": "Tipo de identificación (Cedula de ciudadania, Cedula extranjera, otra)",
        "personalId": "Número de identificación",
        "especialidad": "Especialidad médica (Ej: Pediatría, Cardiología, etc)"
     }'
```

##### Respuesta exitosa (200 OK):
```bash
{
    "mensaje": "La informacion enviada del doctor se registró correctamente"
}
```

##### Notas:
- El campo token debe ser válido y pertenecer a un usuario con tipo doctor.
- Los campos como   `cell `,  `personalId ` y  `tipoId ` son para completar el perfil profesional del doctor.
- Si ya existe información previa, el backend puede optar por actualizarla o retornar un error dependiendo de la lógica implementada.
- Este paso es importante para que el doctor pueda ser visible y seleccionable por los pacientes dentro del sistema.

### Endpoints principales

- `POST /Api/Registro` - Registro de usuarios
- `POST /Api/Registro/Enviar-codigo` - Enviar código de verificación
- `POST /Api/Registro/Confirmar-codigo` - Confirmar código
- `POST /Api/Login` - Inicio de sesión
- `POST /Api/Tipo-user` - Mostrar tipo de usuarios
- `POST /Api/Informacion-Personal-Paciente` - Registrar informacion personal del paciente
- `POST /Api/Informacion-Personal-Doctor` - Registrar informacion personal del doctor

## Licencia

Este proyecto está licenciado bajo los términos de la **Licencia Pública General GNU v3.0 (GPLv3)**.  
Puedes consultar el texto completo en el archivo [LICENSE](./LICENSE) o en [gnu.org/licenses/gpl-3.0.html](https://www.gnu.org/licenses/gpl-3.0.html).
