#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	char buff[512];
	char buff2[512];

	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");

	// Fem el bind al port
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	
	// escucharemos en el port 9050
	serv_adr.sin_port = htons(9070);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind\n");
	//La cola de peticiones pendientes no podr? ser superior a 4
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen\n");
	
	int i;
	// Atenderemos solo 5 peticione
	for(i=0;i<500;i++){
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexión\n");
		//sock_conn es el socket que usaremos para este cliente
		
		// Ahora recibimos su nombre, que dejamos en buff
		ret=read(sock_conn,buff, sizeof(buff));
		printf ("Recibido\n");
		
		// Tenemos que a?adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		buff[ret]='\0';
		
		//Escribimos el nombre en la consola
		
		printf ("Se ha conectado: %s\n",buff);
		
		
		char *p = strtok( buff, "/");
		int form =  atoi (p);
		p = strtok( NULL, "/");
		int codigo =  atoi (p);
		p = strtok( NULL, "/");
		char nombre[20];
		strcpy (nombre, p);
		char password[20];
		printf ("Codigo: %d, Nombre: %s\n", codigo, nombre);
		
		if (form == 1) { // LogIn
			p = strtok( NULL, "/");
			strcpy (password, p);
			if (codigo == 1) { // Iniciar sesión
				
				MYSQL *conn;
				int err;
				MYSQL_RES *resultado;
				MYSQL_ROW row;
				conn = mysql_init(NULL);
				if (conn==NULL) {
					printf ("Error al crear la conexi\uffc3\uffb3n: %u %s\n", 
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				conn = mysql_real_connect (conn, "localhost","root", "mysql", "Juego",0, NULL, 0);
				if (conn==NULL) {
					printf ("Error al inicializar la conexi\uffc3\uffb3n: %u %s\n", 
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				//char comanda [100] = strcat(strcat("SELECT Participacion.Posicion FROM Jugador, Participacion WHERE Jugador.Nombre = '", nombre), "' AND Jugador.Id = Participacion.Id_J");
				char comanda [200];
				sprintf(comanda, "SELECT Nombre, Contraseña FROM Jugador WHERE Nombre = '%s'", nombre);
				err=mysql_query (conn, comanda);
				if (err!=0) {
					printf ("Error al consultar datos de la base %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				resultado = mysql_store_result (conn);
				row = mysql_fetch_row (resultado);
				if (row == NULL) {
					sprintf(buff2, "%s", "2");
				}
				else {
					if (strcmp(password,row[1]) == 0)
						sprintf(buff2, "%s", "0");
					else
						sprintf(buff2, "%s", "1");
				}
				
				mysql_close (conn);
				// exit(0);
				
				printf ("%s\n", buff2);
				
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
				
				// Se acabo el servicio para este cliente
				close(sock_conn); 
			}
			if (codigo == 2) { // Registrarse
							
				MYSQL *conn;
				int err;
				MYSQL_RES *resultado;
				MYSQL_ROW row;
				conn = mysql_init(NULL);
				if (conn==NULL) {
					printf ("Error al crear la conexi\uffc3\uffb3n: %u %s\n", 
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				conn = mysql_real_connect (conn, "localhost","root", "mysql", "Juego",0, NULL, 0);
				if (conn==NULL) {
					printf ("Error al inicializar la conexi\uffc3\uffb3n: %u %s\n", 
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				char comanda [200];
				sprintf(comanda, "SELECT Nombre, Contraseña FROM Jugador WHERE Nombre = '%s'", nombre);
				err=mysql_query (conn, comanda);
				if (err!=0) {
					printf ("Error al consultar datos de la base %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				resultado = mysql_store_result (conn);
				row = mysql_fetch_row (resultado);
				if (row == NULL) { // Usuario no existe => insertamos en BBDD
					
					//Miramos el id más grande
					strcpy(comanda, "SELECT MAX(Id) FROM Jugador");
					err=mysql_query (conn, comanda);
					if (err!=0) {
						printf ("Error al consultar datos de la base %u %s\n",
								mysql_errno(conn), mysql_error(conn));
						exit (1);
					}
					resultado = mysql_store_result (conn);
					row = mysql_fetch_row (resultado);
					int id = atoi(row[0]) + 1;
					
					sprintf(comanda, "INSERT INTO Jugador VALUES(%d,'%s','%s')", id, nombre, password);
					err=mysql_query (conn, comanda);
					if (err!=0) {
						printf ("Error al modificar datos de la base %u %s\n",
								mysql_errno(conn), mysql_error(conn));
						exit (1);
					}
					sprintf(buff2, "%s", "0");
				}
				else {
					sprintf(buff2, "%s", "1");
				}
				
				mysql_close (conn);
				// exit(0);
				
				printf ("%s\n", buff2);
				
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
				
				// Se acabo el servicio para este cliente
				close(sock_conn); 
			}
		}
		else if (form == 2) { // Inicio
			if (codigo == 1) { // posición media
				
				MYSQL *conn;
				int err;
				MYSQL_RES *resultado;
				MYSQL_ROW row;
				conn = mysql_init(NULL);
				if (conn==NULL) {
					printf ("Error al crear la conexi\uffc3\uffb3n: %u %s\n", 
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				conn = mysql_real_connect (conn, "localhost","root", "mysql", "Juego",0, NULL, 0);
				if (conn==NULL) {
					printf ("Error al inicializar la conexi\uffc3\uffb3n: %u %s\n", 
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				//char comanda [100] = strcat(strcat("SELECT Participacion.Posicion FROM Jugador, Participacion WHERE Jugador.Nombre = '", nombre), "' AND Jugador.Id = Participacion.Id_J");
				char comanda [200] = "SELECT Participacion.Posicion FROM Jugador, Participacion WHERE Jugador.Nombre = '";
				strcat(comanda,nombre);
				char comanda2 [100] = "' AND Jugador.Id = Participacion.Id_J";
				strcat(comanda,comanda2);
				err=mysql_query (conn, comanda);
				if (err!=0) {
					printf ("Error al consultar datos de la base %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				resultado = mysql_store_result (conn);
				row = mysql_fetch_row (resultado);
				float media;
				if (row == NULL) {
					printf ("No se han obtenido datos en la consulta\n");
					media = 0;
				}
				else{
					int suma = 0;
					int cont = 0;
					while (row !=NULL) {
						suma += atoi (row[0]);
						cont++;
						row = mysql_fetch_row (resultado);
					}
					media = (float)suma/cont;
				}
				mysql_close (conn);
				// exit(0);
				
				sprintf(buff2, "%g", media);
				printf ("%s\n", buff2);
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
				
				// Se acabo el servicio para este cliente
				close(sock_conn); 
			}
			else if (codigo == 2) { //Jugador que ha ganado más veces
				MYSQL *conn;
				int err;
				MYSQL_RES *resultado;
				MYSQL_ROW row;
				conn = mysql_init(NULL);
				if (conn==NULL) 
				{
					printf ("Error al crear la conexi\ufff3n: %u %s\n",mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				conn = mysql_real_connect (conn, "localhost","root", "mysql", "Juego",0, NULL, 0);
				if (conn==NULL) {
					printf ("Error al inicializar la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				
				char consulta [100];
				strcpy (consulta, "SELECT (Jugador.Nombre) FROM (Jugador,Participacion) WHERE Participacion.Posicion = 1 AND Participacion.Id_J = Jugador.Id ORDER BY Jugador.Nombre DESC");
				err=mysql_query (conn, consulta);
				if (err!=0) 
				{
					printf ("Error al consultar datos de la base %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				resultado = mysql_store_result (conn);
				row = mysql_fetch_row (resultado);
				char ganador[20]="";
				int suma = 0;
				if (row == NULL)
					printf ("No se han obtenido datos en la consulta\n");
				else
				{
					char persona_anterior[20];
					strcpy(persona_anterior, row[0]);
					int jug = 0;
					int i = 0;
					while (row!=NULL) 
					{
						char persona[30];
						strcpy(persona, row[0]);
						
						if (strcmp(persona, persona_anterior)==0)
							i++;
						else {
							i=1;
							strcpy(persona_anterior, persona);
						}
						if (i>suma) {
							strcpy(ganador, persona);
							suma=i;
						}
						row = mysql_fetch_row (resultado);
					}
					//printf ("El que mas veces ha ganado es %s y ha ganado %d veces.", ganador, suma);
				}	
				mysql_close (conn);
				//exit(0);
				
				sprintf(buff2, "%s/%d", ganador, suma);
				printf ("%s\n", buff2);
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
				
				// Se acabo el servicio para este cliente
				close(sock_conn); 
			}
			else if (codigo == 3) {
				MYSQL *conn;
				int err;
				// Estructura especial para almacenar resultados de consultas
				MYSQL_RES *resultado;
				MYSQL_ROW row;
				char consulta [80];
				//Creamos una conexion al servidor MYSQL
				conn = mysql_init(NULL);
				if (conn==NULL) {
					printf ("Error al crear la conexi\uffc3\uffb3n: %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				//inicializar la conexion
				conn = mysql_real_connect (conn, "localhost","root", "mysql", "Juego",0, NULL, 0);
				if (conn==NULL) {
					printf ("Error al inicializar la conexi\uffc3\uffb3n: %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				
				strcpy (consulta,"SELECT COUNT(Participacion.Posicion) FROM Jugador,Participacion WHERE Jugador.Nombre = '");
				strcat (consulta, nombre);
				strcat (consulta,"'AND Jugador.Id = Participacion.Id_J AND Participacion.Posicion = 1");
				
				err=mysql_query (conn, consulta);
				if (err!=0) {
					printf ("Error al consultar datos de la base %u %s\n",
							mysql_errno(conn), mysql_error(conn));
					exit (1);
				}
				
				resultado = mysql_store_result (conn);
				
				row = mysql_fetch_row (resultado);
				
				char victorias[20] = "0";
				
				if (row != NULL)
					sprintf(victorias, "%s", row[0] );
				mysql_close (conn);
				
				sprintf(buff2, "%s", victorias);
				printf ("%s\n", buff2);
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
				
				// Se acabo el servicio para este cliente
				close(sock_conn); 
				
			}
		}
	}	
}
