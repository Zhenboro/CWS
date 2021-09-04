<?php
$ident=$_POST["ident"];
$log=$_POST["log"];
if (isset($ident) && isset($log)) {
	$reportName=$ident.".txt";
	$myReport=fopen($reportName, "w") or die("No se pudo hacer eso!");
	fwrite($myReport, $log);
	fclose($myReport);
}
?>