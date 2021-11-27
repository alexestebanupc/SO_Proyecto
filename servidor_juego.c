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

typedef struct{
	int estado;// 0 si esta vacia, 1 si esta la partida en el lobby. 2 si la partida ha empezado
	int num_jugadores;
	Conectado Jugador1;
	Conectado Jugador2;
	Conectado Jugador3;
	Conectado Jugador4;
	//int numeroPartida; //Hacer una variable global para saber por que numero de partida vamos.
}Partida;

typedef Partida TablaPartidas[100];

ListaConectados listaConectados;
TablaPartidas tablaPartidas;

//Funciones de la tabla de partidas

void Inicializar(TablaPartidas tabla){
	//Inicializa la tabla
	int i;
	for(i=0;i<100;i++){
		tabla[i].estado = 0;
		tabla[i].num_jugadores = 0;
		tabla[i].Jugador1.socket = -1;
		tabla[i].Jugador2.socket = -1;
		tabla[i].Jugador3.socket = -1;
		tabla[i].Jugador4.socket = -1;
	}
}

int PrimeraPosicionLibre(TablaPartidas tabla){
	// Devuelve la posición del primer hueco libre
	int encontrado = 0;
	int i = 0;
	while((i<100)&&(!encontrado)){
		if(tabla[i].estado==0){
			encontrado = 1;
		}
		else
		   i=i+1;
	}
	if (!encontrado){
		return -1;//No quedan huecos libres
	}
	else{
		return i;//Primera posicion libre
	}
	
}

int PonerJugador(TablaPartidas tabla, char nombre[30], int socket, int res){
	//Poner jugador y su socket
	// 0 si se guarda correctamente
	// -1 no quedan partidas libres
	// -2 ya hay 4 jugadores
	if(res!=-1){//Si quedan partidas libres
		
		if(tabla[res].num_jugadores == 0){
			strcpy(tabla[res].Jugador1.nombre,nombre);
			tabla[res].Jugador1.socket = socket;
			tabla[res].num_jugadores = 1;
			tabla[res].estado = 1;
			return 0;
		}
		else if(tabla[res].num_jugadores == 1){
			strcpy(tabla[res].Jugador2.nombre,nombre);
			tabla[res].Jugador2.socket = socket;
			tabla[res].num_jugadores = 2;
			return 0;
		}
		else if(tabla[res].num_jugadores == 2){
			strcpy(tabla[res].Jugador3.nombre,nombre);
			tabla[res].Jugador3.socket = socket;
			tabla[res].num_jugadores = 3;
			return 0;
		}
		else if(tabla[res].num_jugadores == 3){
			strcpy(tabla[res].Jugador4.nombre,nombre);
			tabla[res].Jugador4.socket = socket;
			tabla[res].num_jugadores = 4;
			return 0;
		}
		else
			return -2; // Ya hay 4 jugadores
	}
	else
	   return -1; //No quedan partidas libres
}

int BorrarPartida(TablaPartidas tabla, int indice){
	
	//
	tabla[indice].estado = 0;
	tabla[indice].num_jugadores = 0;
	tabla[indice].Jugador1.socket = -1;
	tabla[indice].Jugador2.socket = -1;
	tabla[indice].Jugador3.socket = -1;
	tabla[indice].Jugador4.socket = -1;
	strcpy(tabla[indice].Jugador1.nombre,"");
	strcpy(tabla[indice].Jugador2.nombre,"");
	strcpy(tabla[indice].Jugador3.nombre,"");
	strcpy(tabla[indice].Jugador4.nombre,"");
}

int BuscarHost(TablaPartidas tabla, char emisor[20]){
	int encontrado = 0;
	int i = 0;
	while((i<100)&&(!encontrado)){
		if(strcmp(tabla[i].Jugador1.nombre,emisor)==0){
			encontrado = 1;
		}
		else
		   i=i+1;
	}
	if (!encontrado){
		return -1;//No quedan huecos libres
	}
	else{
		return i;//Primera posicion libre
	}
}
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
int sockets[MAX]; //no limitRLO a 100 y el de threads tampoco

void Codigo1_1(char nombre[20], char password[20], char buff2[512], char usuario[20], int sock_conn) { 
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
			AnadirConectado(&listaConectados, nombre, sock_conn); //Añadir conectado
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

void Codigo2_5(char emisor[20], char receptor[20], char buff2[512], int sock_conn) {
	//Invitar usuario
	//0 Enviamos el mensaje al receptor
	//-1 Enviamos un aviso del problema al emisor(No hay lugares libres en la tabla).
	//-2 Enviamos un aviso del problema al emisor(Estan los 4 jugadores llenos).
	
	int ind = BuscarHost(tablaPartidas, emisor);
	if(ind==-1){
		int res = PrimeraPosicionLibre(tablaPartidas);
		int socket_receptor = GetSocket(&listaConectados, receptor);
		int aux = PonerJugador(tablaPartidas, emisor, sock_conn, res);
		
		if(aux == 0){
			sprintf(buff2, "2/5/0,%s,%d", emisor, res);
			//Y lo enviamos
			write(socket_receptor,buff2, strlen(buff2));
			printf ("%s\n", buff2);
			sprintf(buff2, "2/5/3,%d", res);
			write(sock_conn, buff2,strlen(buff2));//Enviamos el indice al emisor
		}
		else if(aux == -1){
			sprintf(buff2, "2/5/1");
			//Y lo enviamos
			write(sock_conn,buff2, strlen(buff2));
		}
		else if(aux == -2){
			sprintf(buff2, "2/5/2");
			write(sock_conn,buff2, strlen(buff2));
		}
	}
	else{
		int socket_receptor = GetSocket(&listaConectados, receptor);
		sprintf(buff2, "2/5/0,%s,%d", emisor, ind);
		//Y lo enviamos
		write(socket_receptor,buff2, strlen(buff2));
		printf ("%s\n", buff2);
		sprintf(buff2, "2/5/3,%d", ind);
		write(sock_conn, buff2,strlen(buff2));//Enviamos el indice al emisor
	}
	
	// exit(0);
	printf ("%s\n", buff2);
}

void Codigo2_6(char resultado[20] , int indice, char buff2[512], char usuario[20], int sock_conn){
	// Respuesta invitacion
	//0 Enviamos el mensaje al receptor
	//-1 Enviamos un aviso del problema al emisor(No hay lugares libres en la tabla).
	//-2 Enviamos un aviso del problema al emisor(Estan los 4 jugadores llenos).
	//3 El tiempo ha expirado y no puede entrar en la partida
	
	char receptor[20];
	strcpy(receptor,tablaPartidas[indice].Jugador1.nombre);
	int socket_receptor = GetSocket(&listaConectados, receptor);
	
	if(strcmp(resultado,"si")==0){
		
		if(tablaPartidas[indice].estado == 1){// Si la partida aun no ha empezado ponemos el jugador
			int aux = PonerJugador(tablaPartidas, usuario, sock_conn, indice);
			if(aux == 0){
				sprintf(buff2, "2/6/0,%s,%d,%s", resultado, indice, usuario);
				write (socket_receptor, buff2, strlen(buff2));
			}
			else if(aux == -1){
				sprintf(buff2, "2/6/1"); 
				//Y lo enviamos
				write(sock_conn,buff2, strlen(buff2));
			}
			else if(aux == -2){
				sprintf(buff2, "2/6/2");
				write(sock_conn,buff2, strlen(buff2));
			}
		}
		else{
			sprintf(buff2, "2/6/3"); //
			//Y lo enviamos
			write(sock_conn, buff2, strlen(buff2));
		}
	}
	else{
		sprintf(buff2, "2/6/0,%s,%d,%s", resultado, indice, usuario);
		write (socket_receptor, buff2, strlen(buff2));
	}
	printf ("%s\n", buff2);
}

void Codigo2_7(int indice, char buff2[512], int sock_conn){
	// Han pasado 10 segundos desde la primera invitacion
	//0 Enviamos el mensaje a todos los jugadores que hayan aceptado la invitación antes de los 10 segundos
	//1 Enviamos el mensaje al emisor de que nadie ha aceptado su partida
	
	if(tablaPartidas[indice].Jugador2.socket != -1){//Almenos hay 2 jugadores en la partida, así que podrá empezar
		tablaPartidas[indice].estado = 2;

		for(int d = 1;d <= tablaPartidas[indice].num_jugadores;d++){
			strcpy(buff2,"2/7/0");
			sprintf(buff2,"%s,%d,%d",buff2,tablaPartidas[indice].num_jugadores,d);
			if(d==1){
				write (tablaPartidas[indice].Jugador1.socket, buff2, strlen(buff2));
			}
			else if(d==2){
				write (tablaPartidas[indice].Jugador2.socket, buff2, strlen(buff2));
			}
			else if (d==3){
				write (tablaPartidas[indice].Jugador3.socket, buff2, strlen(buff2));
			}
			else{
				write (tablaPartidas[indice].Jugador4.socket, buff2, strlen(buff2));
			}
			printf("%s\n",buff2);
		}
	}
	else { //La partida no empieza porque nadie ha aceptado
		strcpy(buff2,"2/7/1");
		write (tablaPartidas[indice].Jugador1.socket, buff2, strlen(buff2));
		BorrarPartida(tablaPartidas,indice);
	}
}

void Codigo2_8 (char usuario[20], char buff2[512], int sock_conn, char texto[200]){
	//Envia el mensaje del chat a todos los conectados
	sprintf(buff2, "2/8/%s,%s", usuario, texto);
	int j;
	for (j=0;j<i;j++){
		write(sockets[j],buff2,strlen(buff2));//enviamos el mensaje de chat
	}
	printf ("\n%s\n", buff2);
}

void Codigo2_9(char buff2[512], int sock_conn, int indicePartida, char idCoche[20], char posX[20], char posY[20]){
	
	sprintf(buff2,"2/9/%s,%s,%s", idCoche, posX, posY);
	
	if(tablaPartidas[indicePartida].Jugador1.socket != -1 && tablaPartidas[indicePartida].Jugador1.socket != sock_conn){
		write(tablaPartidas[indicePartida].Jugador1.socket,buff2,strlen(buff2));
	}
	if(tablaPartidas[indicePartida].Jugador2.socket != -1 && tablaPartidas[indicePartida].Jugador2.socket != sock_conn){
		write(tablaPartidas[indicePartida].Jugador2.socket,buff2,strlen(buff2));
	}
	if(tablaPartidas[indicePartida].Jugador3.socket != -1 && tablaPartidas[indicePartida].Jugador3.socket != sock_conn){
		write(tablaPartidas[indicePartida].Jugador3.socket,buff2,strlen(buff2));
	}
	if(tablaPartidas[indicePartida].Jugador4.socket != -1 && tablaPartidas[indicePartida].Jugador4.socket != sock_conn){
		write(tablaPartidas[indicePartida].Jugador4.socket,buff2,strlen(buff2));
	}
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
		if (form!=0 && codigo != 8 && codigo !=9){
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
				Codigo1_1(nombre, password, buff2, usuario, sock_conn);
				// Y lo enviamos
				write (sock_conn,buff2, strlen(buff2));
				printf("Jug: %s; socket: %d", usuario, sock_conn);
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
			else if (codigo == 5){// Enviar invitación
				pthread_mutex_lock( &mutex );
				Codigo2_5(usuario, nombre, buff2, sock_conn);
				pthread_mutex_unlock( &mutex );
			}
			else if (codigo == 6){// Recibir invitación
				
				char respuesta[20];
				strcpy(respuesta, nombre);
				p = strtok(NULL,"/");
				int indice = atoi(p);
				pthread_mutex_lock( &mutex );
				Codigo2_6(respuesta, indice, buff2, usuario, sock_conn);
				pthread_mutex_unlock( &mutex );
			}
			else if (codigo==7){// Notificar si la partida ha empezado
				pthread_mutex_lock( &mutex );
				Codigo2_7(atoi(nombre), buff2, sock_conn);
				pthread_mutex_unlock( &mutex );
			}
			else if (codigo==8){// Recibir mensaje del chat
				char texto[200];
				p = strtok(NULL,"/");
				strcpy (texto, p);
				Codigo2_8(usuario, buff2, sock_conn, texto);
			}
			else if(codigo==9){
				int indicePartida;
				p=strtok(NULL,",");
				indicePartida = atoi(p);
				char idCoche[20];
				p = strtok(NULL,",");
				strcpy (idCoche, p);
				char posX[20];
				p = strtok(NULL,",");
				strcpy (posX, p);
				char posY[20];
				p = strtok(NULL,",");
				strcpy (posY, p);
				Codigo2_9(buff2, sock_conn, indicePartida, idCoche, posX, posY);
			}
		}
		if(((form==0)&&(codigo==0))||((form==1)&&(codigo==1))){ //Cada vez que un usuario se conecta o desconecta enviamos una notificación de la lista de conectados actualizada a el mismo y al resto de usuarios
			Cadena(&listaConectados, notificacion);
			sprintf(buff2, "2/4/%s", notificacion);
			int j;
			for (j=0;j<i;j++){
				write(sockets[j],buff2,strlen(buff2));//Cambiar los sockets por los sockets de la lista
			}
			printf ("\n%s\n", buff2);
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
	serv_adr.sin_port = htons(9060);//************************************************************************
	//serv_adr.sin_port = htons(50054);
	int error_bind=0;
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0){
		printf ("Error al bind\n");
		error_bind=1;
	}
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
	//conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T2Juego",0, NULL, 0);
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Juego",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexi\uffc3\uffb3n: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	Inicializar(tablaPartidas);
	i=0;
	
	for(;;){
		if (error_bind==0)
			printf ("Escuchando\n");
	
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexión\n");
		//sock_conn es el socket que usaremos para este cliente
		
		sockets[i] = sock_conn;
		//Atender
		pthread_create (&thread[i], NULL, AtenderCliente, &sockets[i]);
		i++;
	}
	mysql_close (conn);
}
