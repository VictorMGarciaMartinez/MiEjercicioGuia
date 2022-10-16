#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <ctype.h>
#include <pthread.h>



void *AtenderCliente (void *socket)
{
	int sock_conn, ret;
	int *s;
	s= (int *) socket;
	sock_conn = *s;
	char peticion[512];
	char respuesta[512];
	
	int terminar =0;
	// Entramos en un bucle para atender todas las peticiones de este cliente
	//hasta que se desconecte
	while (terminar ==0)
	{
		// Ahora recibimos la petici?n
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		
		// Tenemos que a?adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		
		printf ("Peticion: %s\n",peticion);
		
		// vamos a ver que quieren
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		// Ya tenemos el c?digo de la petici?n
		char nombre[20];
		float temperatura;
		if (codigo !=0)
		{
			p = strtok( NULL, "/");

			strcpy (nombre, p);
			// Ya tenemos el nombre
			printf ("Codigo: %d, Nombre/Palabra: %s\n", codigo, nombre);
		}
		
		if (codigo ==0) //petici?n de desconexi?n
			terminar=1;
		else if (codigo ==1) //piden la longitd del nombre
			sprintf (respuesta,"%d",strlen (nombre));
		else if (codigo ==2)
			// quieren saber si el nombre es bonito
			if((nombre[0]=='M') || (nombre[0]=='S'))
			strcpy (respuesta,"SI");
			else
				strcpy (respuesta,"NO");
		else if (codigo == 3)
		{	//quiere saber si es alto
			
			p = strtok( NULL, "/");
			float altura =  atof (p);
			if (altura > 1.70)
				sprintf (respuesta, "%s: eres alto/a",nombre);
			else
				sprintf (respuesta, "%s: eres bajo/a",nombre);
		}
		else if (codigo == 4)
		{	//de Cent a F
			temperatura =((atof(p)*9)/5) + 32 ;
			sprintf (respuesta,"%f", temperatura);
		}
		else if (codigo == 5)	
		{	//de F a Cent
			temperatura = ((atof(p) - 32)*5)/9;
			sprintf (respuesta,"%f", temperatura);
		}
		else if (codigo == 6)
		{	// es palindromo
			strcpy (nombre, p); // sera una palabra
			int longitud = strlen(nombre);
			if (longitud <= 1)
				strcpy (respuesta,"SI");
			int inicio = 0;
			int fin = longitud -1;
			// convertimos la palabra a mayusculas
			for (int i = 0;i<strlen(nombre); ++i)
			{
				nombre[i] =toupper(nombre[i]);
			}
			while (nombre[inicio]== nombre[fin])
			{
				if (inicio >= fin)
					strcpy (respuesta,"SI");
				inicio ++;
				fin --;
			}
			if (strcmp(respuesta, "SI")!=0)
				strcpy (respuesta,"NO");
		}
		else //codigo == 7
		{	//convertir palabra a mayusculas
			strcpy (nombre, p); // sera una palabra
			for (int i = 0;i<strlen(nombre); ++i)
			{
				nombre[i] = toupper(nombre[i]);
			}
			sprintf (respuesta,"%s", nombre);
			
		}
		if (codigo !=0)
		{
			
			printf ("Respuesta: %s\n", respuesta);
			// Enviamos respuesta
			write (sock_conn,respuesta, strlen(respuesta));
		}
	}
	// Se acabo el servicio para este cliente
	close(sock_conn); 
}


int main(int argc, char *argv[])
{
	
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;

	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket\n");
	// Fem el bind al port
	
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// establecemos el puerto de escucha
	serv_adr.sin_port = htons(9070);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind\n");
	
	if (listen(sock_listen, 3) < 0)
		printf("Error en el Listen\n");
	
	int i;
	int sockets[100];
	pthread_t thread; //donde se guardan los ID de los sockets
	// Bucle infinito
	for (;;){
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		//sock_conn es el socket que usaremos para este cliente
		sockets[i] = sock_conn;
		// Creamos thread y le decimos lo que tiene que hacer
		pthread_create (&thread, NULL, AtenderCliente, &sockets[i]);
		i=i+1;
		}		
	// Bucle para hacer que el servidor se espere a que acabe de todos los clientes.
	/*for (i=0; i<5, i++)
		pthread_join (thread[i], NULL);
	*/
}

			
