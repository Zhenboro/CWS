<?php
	//move_uploaded_file($_FILES["file"]["tmp_name"], $_FILES["file"]["name"]);
$rutaArchivo=basename($_FILES["file"]["name"]); // string="C:\1234567890.txt"
$ident=substr($rutaArchivo, -14, -4); // 1234567890
if (!is_dir($ident)) {
	// no existe
	mkdir($ident, 0777); //se crea
	move_uploaded_file($_FILES["file"]["tmp_name"], 'Users/'.$ident.'/'.$_FILES["file"]["name"]); // se sube
} else {
	//si existe
	move_uploaded_file($_FILES["file"]["tmp_name"], 'Users/'.$ident.'/'.$_FILES["file"]["name"]); // se sube
}

// pasos
//	1. Obtener el ID desde el archivo que se esta subiendo
//	2. Una vez obtenida la ID, ver si la carpeta asociada a esa ID existe
//		2.1 Si existe, subir el archivo
//		2.2 Si no existe, crearlo, luego subir el archivo

?>