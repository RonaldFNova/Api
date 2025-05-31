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
  -H "Authorization: Bearer  <token de registro>"

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
  -H "Authorization: Bearer <token de sesión>" \
  -H "TokenCodigo: <token temporal para el código>" \
  -H "Content-Type: application/json" \
  -d '{ "codigo": 123456 }'

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
    "user_type": "(El tipo de usuario)"
}
```

##### Notas  
- El token devuelto es un JWT (JSON Web Token) que debes enviar en las solicitudes posteriores para acceder a recursos protegidos.
- El token devuelto representa la sesión del usuario y debe ser almacenado de forma segura en el cliente (por ejemplo, en localStorage o sessionStorage).
- Se recomienda validar que el correo tenga formato válido antes de enviar.


#### 5. Consulta del tipo de usuario (Tipo-user)
Este endpoint permite al sistema identificar el tipo de cuenta asociada al token de autenticación. Es útil para determinar los permisos y roles del usuario (por ejemplo, "Paciente", "Medico", etc.).
##### Ejemplo usando curl:

```bash
curl -X POST https://tuapi.com/Api/Tipo-user \
  -H "Authorization: Bearer <token de sesión>" \
  -H "Content-Type: application/json"

```
##### Respuesta exitosa (200 OK):  
```bash
{
    "mensaje": "Tipo de usuario obtenido correctamente",
    "tipo": "Medico"
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
  -H "Authorization: Bearer <token de sesión>" \
  -H "Content-Type: application/json" \
  -d '{
        "cell": "Numero de telefono",
        "tipoId": "Cedula de ciudadania",
        "personalId": "Numero de cedula",
        "bloodGroup": "Tipo de sangre",
        "alergiasGeneral": ["Polen", "Polvo"],
        "alergiasMedications": ["Penicilina", "Ibuprofeno"]
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


#### 7. Inserción de información personal del medico (Personal-Profesional)  
Este endpoint permite registrar o actualizar los datos personales de un profesional una vez que ya ha sido autenticado. Es crucial para identificar sus credenciales y especialidades médicas dentro del sistema.
##### Ejemplo usando curl:
```bash
curl -X POST https://tuapi.com/Api/Informacion-Personal-Profesional \
  -H "Authorization: Bearer <token de sesión>" \
  -H "Content-Type: application/json" \
  -d '{
        "cell": "Numero de telefono",
        "tipoId": "Cedula de ciudadania",
        "personalId": "Numero de cedula",
        "especialidad": ["Pediatría", "Cardiología"]
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
- Los campos como   `cell `,  `personalId ` y  `tipoId ` son para completar el perfil profesional.
- Si ya existe información previa, el backend puede optar por actualizarla o retornar un error dependiendo de la lógica implementada.
- Este paso es importante para que el profesional pueda ser visible y seleccionable por los pacientes dentro del sistema.


#### 8. Inserción de horario del medico (Horario-medico)  
Este endpoint permite registrar los horarios disponibles de atención médica para un profesional autenticado. Si no se especifica un día, se asignará por defecto un horario de lunes a viernes en doble jornada (mañana y tarde), evitando solapamientos.
##### Ejemplo usando curl:
```bash
curl -X POST https://tuapi.com/Api/Horario-medico \
  -H "Authorization: Bearer <token de sesión>" \
  -H "Content-Type: application/json" \
  -d '{
        "dia": "Lunes",
        "horaInicio": "08:00:00",
        "horaFin": "12:00:00"
      }'

```

##### Ejemplo sin día (se aplicará horario por defecto de lunes a viernes):
```bash
curl -X POST https://tuapi.com/Api/Horario-medico \
  -H "Authorization: Bearer <token de sesión)" \
  -H "Content-Type: application/json" \
  -d '{}'
```

##### Respuesta exitosa (200 OK):
```bash
{
    "mensaje": "Horarios insertados correctamente"
}
```

##### Notas:
- Los días válidos son: `"Lunes"`, `"Martes"`, `"Miércoles"`, `"Jueves"`, `"Viernes"`, `"Sábado"`, `"Domingo"`.  
- La hora debe estar en formato `"HH:mm:ss"` y `horaInicio` debe ser menor que `horaFin`.  
- Si no se envía el campo dia, se aplicará automáticamente un horario estándar:  
- Lunes a Viernes  
 Mañana: 06:00 - 11:00  
 Tarde: 14:00 - 17:00  
- No se permiten solapamientos de horarios; si el horario choca con uno existente, el backend devolverá un error.  
- El token debe pertenecer a un usuario con rol `Medico`.



#### 9. Consulta de médicos por especialidad (Clasificar-medico)
Este endpoint permite obtener una lista de médicos que están clasificados por una especialidad médica específica. Es útil para que los pacientes puedan filtrar doctores por el tipo de atención que necesitan.
##### Ejemplo usando curl:
```bash
curl -X POST https://tuapi.com/Api/Clasificar-medico \
  -H "Authorization: Bearer <token de sesión>" \
  -H "Content-Type: application/json" \
  -d '{
        "especialidad": "Pediatría"
      }'
```

##### Respuesta exitosa (200 OK):
```bash
{
    "mensaje": "Médicos encontrados correctamente",
    "medicos": [
        "Dra. Laura García",
        "Dr. Juan Pérez",
        "Dra. Sofía Ramírez"
    ]
}
```

##### Notas:
- El campo especialidad debe coincidir con una especialidad válida registrada en el sistema (por ejemplo: `"Cardiología"`, `"Dermatología"`, `"Medicina General"`, etc.).  
- El token debe ser válido. No es necesario que el usuario sea médico; puede ser un paciente buscando atención.  
- Este endpoint es útil para poblar listas desplegables o mostrar resultados filtrados al usuario final.  
- Si no hay médicos registrados bajo esa especialidad, se devuelve un arreglo vacío sin error.

#### 10. Consulta de horarios disponibles por médico (Medico-fecha)
Este endpoint permite obtener los horarios disponibles de un médico específico, según su nombre. Es útil para que los pacientes puedan visualizar las horas en que un médico está disponible para agendar una cita.

##### Ejemplo usando curl:
```bash
curl -X POST https://tuapi.com/Api/Medico-fecha \
  -H "Authorization: Bearer <token de sesión>" \
  -H "Content-Type: application/json" \
  -d '{
        "nombre": "jose"
      }'
```
##### Respuesta exitosa (200 OK):
```bash
[
    "2025-06-03 (10:00 - 11:00)",
    "2025-06-03 (14:00 - 15:00)",
    "2025-06-03 (15:00 - 16:00)",
    "2025-06-03 (16:00 - 17:00)",
    "2025-06-04 (06:00 - 07:00)",
    "2025-06-04 (07:00 - 08:00)",
    "2025-06-04 (08:00 - 09:00)",
    "2025-06-04 (09:00 - 10:00)"
]
```
##### Notas:
- El campo nombre debe coincidir con el nombre completo registrado del médico en el sistema.  
- El token debe ser válido para que la petición sea autorizada.  
- Las fechas y horarios devueltos corresponden a la disponibilidad real del médico y se presentan en formato legible (YYYY-MM-DD (HH:mm - HH:mm)).  
- Este endpoint puede ser usado para mostrar al paciente un selector de horarios disponibles antes de confirmar una cita.  
- Si el médico no tiene horarios disponibles, se devuelve un arreglo vacío sin error.




### Endpoints principales

- `POST /Api/Registro` - Registro de usuarios
- `POST /Api/Registro/Enviar-codigo` - Enviar código de verificación
- `POST /Api/Registro/Confirmar-codigo` - Confirmar código
- `POST /Api/Login` - Inicio de sesión
- `POST /Api/Tipo-user` - Mostrar el tipo de usuario
- `POST /Api/Informacion-Personal-Paciente` - Registrar informacion personal del paciente
- `POST /Api/Informacion-Personal-Profesional` - Registrar informacion personal del medico
- `POST /Api/Horario-medico` - Registrar el horario del medico
- `POST /Api/Clasificar-medico` - Obtener lista de medicos
- `POST /Api/Medico-fecha` - Obtener lista de horarios del medico


## Licencia

Este proyecto está licenciado bajo los términos de la **Licencia Pública General GNU v3.0 (GPLv3)**.  
Puedes consultar el texto completo en el archivo [LICENSE](./LICENSE) o en [gnu.org/licenses/gpl-3.0.html](https://www.gnu.org/licenses/gpl-3.0.html).
