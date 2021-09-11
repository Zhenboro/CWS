<?php
$ident=$_POST["ident"];
$log=$_POST["log"];
if (isset($ident) && isset($log)) {
	$reportName='Users/'.$ident.".txt";
	// el formato de nombre es ej: xxXxXx.txt
	if (file_exists($reportName)) {
		// existe, entonces sobreescribir
		file_put_contents($reportName, $log);
	} else {
		// no existe, se crea y escribe
		$myReport=fopen($reportName, "w") or die("No se pudo hacer eso!");
		fwrite($myReport, $log);
		fclose($myReport);
	}
}
?>