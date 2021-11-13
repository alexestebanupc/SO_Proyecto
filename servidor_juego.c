#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>

#define MAX 100

typedef struct {
	char nombre[20];
	int socket;
} Conectado;

typedef struct {
	int num;
	Conectado conectados[MAX];
} ListaConectados;


ListaConectados listaConectados;

//Funciones de la lista de conectados

int AnadirConectado(ListaConectados *lista, char nombre[20], int socket) {
	// 0 si OK
	if (lista->num <  MAX) {
		
		strcpy(lista->conectados[lista->num].nombre,nombre);
		lista->conectados[lista->num].socket = socket;
		lista->num = lista->num + 1;
		
		return 0;
	}
	else
		return -1;
}

int BorrarConectado(ListaConectados *lista, char nombre[20]) {
	int found = 0;
	
	for (int i = 0; i < lista->num-1; i++) {
		if (!found) {
			if (strcmp(lista->conectados[i].nombre,nombre)==0) {
				found = 1;
				lista->conectados[i]=lista->conectados[i+1];
				lista->num--;
			}
		}
		else{
			lista->conectados[i]=lista->conectados[i+1];
		}
	}
	if (strcmp(lista->conectados[lista->num-1].nombre,nombre)==0) {
		lista->num--;
		found = 1;
		strcpy(lista->conectados[lista->num].nombre,"");
		lista->conectados[lista->num].socket = 0;
	}
	
	if (found)
		return 0;
	else
		return -1;
}

int GetSocket(ListaConectados *lista, char nombre[20]) {
	// -1 si no encuentra ese jugador
	int found = 0;
	int i = 0;
	int socket= -1;
	while (i < lista->num && !found) {
		if (strcmp(lista->conectados[i].nombre,nombre)==0) {
			socket = lista->conectados[i].socket;
			found = 1;
		}
		i++;
	}
	return socket;
}

void EscribeLista(ListaConectados *lista){
	printf("Número de jugadores: %d\n", lista->num);
	for (int i = 0; i < lista->num; i++)
		printf("\tJugador número %d -> nombre %s con socket %d\n", i, lista->conectados[i].nombre, lista->conectados[i].socket);
}

void Cadena(ListaConectados *lista, char cadena[200]) {
	// numero de jugadores + nombre de todos esos jugadores, separados todos esos datos por una coma
	sprintf(cadena, "%d,", lista->num);
	printf("%d",lista->num);
	for (int i = 0; i < lista->num; i++){
		sprintf(cadena, "%s%s,", cadena, lista->conectados[i].nombre);
		printf("%s",lista->conectados[i].nombre);
	}
	cadena[strlen(cadena)-1] = '\0';
}

void Entrega(ListaConectados *lista, char cadena[200], char nombres[200]) {
	// recibe un vector nombres de jugadores separados por comas y devuelve cadena con los sockets de cada uno de estos jugadores, tambiÃ©n separados por comas.
	char *p;
	p = strtok(nombres, ",");
	int socket;
	
	if (p != NULL) {
		socket = GetSocket(lista, p);
		sprintf(cadena, "%d", socket);
		p = strtok(NULL, ",");
	}
	
	while (p != NULL) {
		socket = GetSocket(lista, p);
		sprintf(cadena, "%s,%d,", cadena, socket);
		p = strtok(NULL, ",");
		cadena[strlen(cadena)-1] = '\0';
	}
}


//Funciones del servidor

MYSQL *conn;
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER; 
int i;
int sockets[MAX];

void Codigo1_1(char nombre[20], char password[20], char buff2[512], char usuario[20]) { 
	// Iniciar sesión.
	// 0 Usuario loggeado
	// 1 Contraseña incorrecta.
	// 2 Usuario no registrado.
	char comanda[200];
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
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
		sprintf(buff2, "1/1/%s", "2"); //Usuario no registrado
	}
	else {
		if (strcmp(password,row[1]) == 0){
			
			sprintf(buff2, "1/1/%s", "0"); //Usuario loggeado
			pthread_mutex_lock( &mutex );
			AnadirConectado(&listaConectados, nombre, socket); //Añadir conectado
			pthread_mutex_unlock( &mutex );
			strcpy(usuario,nombre);
		}
		else
			sprintf(buff2, "1/1/%s", "1"); //Contraseña incorrecta.
	}
	printf ("%s\n", buff2);
}

void Codigo1_2(char nombre[20], char password[20], char buff2[512]) {
	//Registrar usuario
	// 0 Usuario registrado correctamente
	// 1 Usuario ya existe
	char comanda[200];
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
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
		sprintf(buff2, "1/2/%s", "0");
	}
	else {
		sprintf(buff2, "1/2/%s", "1");
	}
	
	// exit(0);
	
	printf ("%s\n", buff2);
}

void Codigo2_1(char nombre[20], char password[20], char buff2[512]) {
	//Devuelve la posicin media en la que ha quedado un jugador
	char comanda[200];
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	sprintf(comanda, "SELECT Participacion.Posicion FROM Jugador, Participacion WHERE Jugador.Nombre = '%s' AND Jugador.Id = Participacion.Id_J", nombre);
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
	// exit(0);
	
	sprintf(buff2, "2/1/%g", media);
	printf ("%s\n", buff2);
}

void Codigo2_2(char nombre[20], char password[20], char buff2[512]) {
	// Devuelve el nombre del usuario que ha ganado más veces y el numero de victorias
	char comanda[200];
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	sprintf(comanda, "SELECT (Jugador.Nombre) FROM (Jugador,Participacion) WHERE Participacion.Posicion = 1 AND Participacion.Id_J = Jugador.Id ORDER BY Jugador.Nombre DESC");
	err=mysql_query (conn, comanda);
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
	//exit(0);
	
	sprintf(buff2, "2/2/%s,%d", ganador, suma);
	printf ("%s\n", buff2);
}

void Codigo2_3(char nombre[20], char password[20],  char buff2[512]) {
	//Veces que ha ganado el usuario que pides.
	char comanda[200];
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	strcpy (comanda,"SELECT COUNT(Participacion.Posicion) FROM Jugador,Participacion WHERE Jugador.Nombre = '");
	strcat (comanda, nombre);
	strcat (comanda,"'AND Jugador.Id = Participacion.Id_J AND Participacion.Posicion = 1");
	
	err=mysql_query (conn, comanda);
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
	
	
	sprintf(buff2, "2/3/%s", victorias);
	printf ("%s\n", buff2);
}

void *AtenderCliente(void *socket){
	
	int sock_conn;
	int *s;
	s = (int *) socket;
	sock_conn = *s;
	// int sock_conn =  (int ) socket;
	
	char buff[512];
	char buff2[512];
	int ret;
	int terminar = 0;
	while (terminar == 0){
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
		int codigo;
		p = strtok( NULL, "/");
		codigo =  atoi (p);
		char nombre[20];
		char usuario[20];
		char password[20];
		char notificacion[200];
		if (form!=0){
			p = strtok( NULL, "/");
			strcpy (nombre, p);
			printf ("Codigo: %d, Nombre: %s\n", codigo, nombre);
		}
		if (form == 0){ //Nos desconectamos del servidor
			if (codigo==0){
				terminar = 1;
				pthread_mutex_lock( &mutex );
				BorrarConectado(&listaConectados,usuario);
				pthread_mutex_unlock( &mutex );
			}
		}
		else if (form == 1) { // LogIn
			p = strtok( NULL, "/");
			strcpy (password, p);
			if (codigo == 1) { // Iniciar sesión
				Codigo1_1(nombre, password, buff2, usuario);
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
				
			}
			if (codigo == 2) { // Registrarse
				
				Codigo1_2(nombre, password, buff2);
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
			}
		}
		else if (form == 2) { // Inicio
			if (codigo == 1) { // posición media
				
				Codigo2_1(nombre, password, buff2);
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
			}
			else if (codigo == 2) { //Jugador que ha ganado más veces
				
				Codigo2_2(nombre, password, buff2);
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
			}
			else if (codigo == 3) { //Numero victorias de x jugador
				
				Codigo2_3(nombre, password, buff2); 
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
			}
		}
		if(((form==0)&&(codigo==0))||((form==1)&&(codigo==1))){ //Cada vez que un usuario se conecta o desconecta enviamos una notificación de la lista de conectados actualizada a el mismo y al resto de usuarios
			Cadena(&listaConectados, notificacion);
			sprintf(buff2, "2/4/%s", notificacion);
			int j;
			for (j=0;j<i;j++){
				write(sockets[j],buff2,strlen(buff2));
			}
		}
	}
}

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
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
	serv_adr.sin_port = htons(50054);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind\n");
	//La cola de peticiones pendientes no podr? ser superior a 4
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen\n");
	

	pthread_t thread[MAX];
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexi\uffc3\uffb3n: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T2Juego",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexi\uffc3\uffb3n: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	// Atenderemos solo 5 peticione
	for(i=0;i<500;i++){
		printf ("Escuchando\n");
	
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexión\n");
		//sock_conn es el socket que usaremos para este cliente
		
		sockets[i] = sock_conn;
		//Atender
		pthread_create (&thread[i], NULL, AtenderCliente, &sockets[i]);
	}
	mysql_close (conn);
}
