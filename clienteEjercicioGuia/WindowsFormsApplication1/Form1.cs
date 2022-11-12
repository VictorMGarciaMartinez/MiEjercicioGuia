using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets; // libreria de sockets
using System.Threading; // libreria de threads


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server; // declaramos socket
        Thread atender; // declaramos thread

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false; // que permita modificar la label los threads.
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }
        // Creamos funcion del thread
        private void AtenderServidor()
        {
            // Bucle infinito
            while (true)
            {
                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                // Partimos el mensaje por la "/"
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje = trozos[1].Split('\0')[0];
                switch (codigo)
                {
                    case 1: //respuesta a longitud
                            MessageBox.Show("La longitud de tu nombre es: " + mensaje);
                            break;
                    case 2: // respuesta a si nombre es bonito
                            if (mensaje == "SI")
                                MessageBox.Show("Tu nombre ES bonito.");
                            else
                                MessageBox.Show("Tu nombre NO bonito. Lo siento.");
                            break;
                    case 3: //respuesta a si alto
                            MessageBox.Show(mensaje);
                            break;
                    case 4: //respuesta a temperatura a F
                            MessageBox.Show("La temperatura de " + palabra_box.Text + "ºC es " + mensaje + " F");
                            break;
                    case 5: //respuesta a Temperatura a ºC
                            MessageBox.Show("La temperatura de " + palabra_box.Text + "F es " + mensaje + " ºC");
                            break;
                    case 6: //respuesta a palíndromo
                            MessageBox.Show("La palabra " + palabra_box.Text + " " + mensaje + " es palindroma.");
                            break;
                    case 7: //respuesta a mayúsculas
                            MessageBox.Show(mensaje);
                            break;
                    case 8: // notificacion
                            serviciosLbl.Text = mensaje;
                            break;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9070);
            

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Conectado");

            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

            // Inicializamos el thread que atendera los mensajes del servidor
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (palabra_box.Text.Length != 0)
            { 
                if (Longitud.Checked)
                {
                    string mensaje = "1/" + palabra_box.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                else if (Bonito.Checked)
                {
                    string mensaje = "2/" + palabra_box.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);                

                }
                else if (altura.Checked)
                {
                    // Enviamos nombre y altura
                    string mensaje = "3/" + palabra_box.Text + "/" + palabra_box.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                }
                else if (aFahrenheit.Checked)
                {
                    string mensaje = "4/" + palabra_box.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                }
                else if (aCentigrados.Checked)
                {
                    string mensaje = "5/" + palabra_box.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                else if (PalabraPalindromo.Checked)
                {
                    string mensaje = "6/" + palabra_box.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                }
                else //if (ConvertirMayus.Checked)
                {
                    // Enviamos palabra
                    string mensaje = "7/" + palabra_box.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";
        
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            atender.Abort(); // cerramos thread
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();

        }

    }
}
