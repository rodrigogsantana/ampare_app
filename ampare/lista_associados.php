 <?php

header("Access-Control-Allow-Origin: *");
header('Content-Type: text/html; charset=utf-8');

	require 'connection.php';
	 
   //fetch table rows from mysql db
    $sql = "select * from id3064554_ampare";
    $result = mysqli_query($connection, $sql) or die("Não foi possível estabelecer conexão com a base de dados ao selecionar " . mysqli_error($connection));

    //create an array
    $associados = array();
    
    if($result)
{
    while($row = mysqli_fetch_array($result))
    {
        $associados[] = $row;
    }	
	
	 print (json_encode($associados));
	 
	 //close the db connection
}
    mysqli_close($connection);

    ?>