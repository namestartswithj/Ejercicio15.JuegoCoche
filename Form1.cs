﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Ejercicio15.JuegoCoche
{
    public partial class Pista : Form
    {
        public Pista()
        {
            InitializeComponent();
        }
        #region Variables y objetos globales
        // Variables para aumentar el tamaño del objeto cuando se detecta una colisión
        public int inflateX = 2;
        public int inflateY = 2;
        // Variable tipo enum para definir la posición o estado actual del auto
        enum EstadoActualAuto
        {
            izquierdo,
            derecho
        }
        Graphics carretera;
        Coche Auto1;
        // Se definen los hilos para recorrer la pista
        Thread th_objAuto;
        // se definen las listas que almacenan los objetos
        List<Circulo> listaCirculoAuto;
        List<Cubo> listaCuboAuto;
        // Se definen los colores que se van a pasar como parámetros para pintar el auto, círculo y cubo
        Color obj1 = Color.Red;
        Color obj2 = Color.GreenYellow;
        // Se define la variable enum para saber de qué lado está el auto
        EstadoActualAuto edo_ActualAuto1;
        // Definir la posición inicial del auto en el lado izquierdo
        const int posInicial_Auto1X = 52;
        // Variable para inicializar el puntaje
        int puntaje;
        #endregion

        #region Métodos del juego
        public void IniciaJuego()
        {
            // Inicializar el puntaje en 0
            puntaje = 0;
            // Se crean las listas para los objetos
            listaCirculoAuto = new List<Circulo>();
            listaCuboAuto = new List<Cubo>();
            // Se crea y se pinta el auto
            Auto1 = new Coche(posInicial_Auto1X, obj2);
            // Posición inicial del auto lo colocaré en el lado izquierdo
            edo_ActualAuto1 = EstadoActualAuto.izquierdo;
            // Generar el primer objeto (círculo y cubo)
            GenerarObjetoAuto1();
            // Se inicializan los timers
            timerCar1.Start(); // Inicializa el auto
            timerGenObjCar1.Start(); // Inicializa los objetos (círculos y cubos)
            // Se asignan los métodos a los threads
            th_objAuto = new Thread(new ThreadStart(colisionCar));
            // Se inicializan los hilos para detectar las colisiones
            th_objAuto.Start();
        }

        // Método que se ejecuta cuando se pierde en el juego (Colisión con obstáculo
        public void Perdio()
        {
            // Se detienen los cronómetros (timers)
            timerCar1.Stop();
            timerGenObjCar1.Stop();
            MessageBox.Show("Has chocado con un obstáculo. Presiona R para volver a jugar", "Juego terminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Método que detecta la colisión y escala los objetos
        public void inflateCubo(Cubo cubo)
        {
            // Realiza una escala de la figura al detectar la colisión
            for (int i = 1; i<5; i++)
            {
                // Reduce la posición de X del objeto
                cubo.X -= (inflateX * i);
                // Aumenta el ancho del objeto
                cubo.inflateX = 2 * (inflateX * i);
                // Lo mismo para Y
                cubo.Y -= (inflateY * i);
                cubo.inflateY = 2 * (inflateY * i);
                // Hacer una pausa de 5ms en el thread
                Thread.Sleep(5);
            }
        }

        public void inflateCirculo(Circulo circulo)
        {
            // Realiza una escala de la figura al detectar la colisión
            for (int i = 1; i < 5; i++)
            {
                // Reduce la posición de X del objeto
                circulo.X -= (inflateX * i);
                // Aumenta el ancho del objeto
                circulo.inflateX = 2 * (inflateX * i);
                // Lo mismo para Y
                circulo.Y -= (inflateY * i);
                circulo.inflateY = 2 * (inflateY * i);
                // Hacer una pausa de 5ms en el thread
                Thread.Sleep(5);
            }
        }

        // Método que genera los objetos en la pantalla
        public void GenerarObjetoAuto1()
        {
            // Creo un objeto random para lanzar los objetos de manera aleatoria en la pantalla
            Random rnd = new Random();
            // La posición de los objetos puede que sea la misma debido al valor generado de los números aleatorios
            // Se amplía el rango de estos valores y se decide si es número par o impar en donde colocar los objetos (izquierda o derecha)
            if (rnd.Next(2, 11) % 2 == 0)
            {
                if (rnd.Next(10, 20) % 2 != 0) // Aparece en la izquierda
                {
                    listaCirculoAuto.Add(new Circulo(new Point(52, 0), obj1));
                }
                else
                {
                    // Es imar y aparece a la derecha
                    listaCirculoAuto.Add(new Circulo(new Point(177, 0), obj1));
                }
            }
            else // Se pintará un cubo
            {
                if (rnd.Next(21, 33) % 2 == 0) // Es un valor random entre 21 y 33, y es par
                {
                    listaCuboAuto.Add(new Cubo(new Point(52, 00), obj2));
                }
                else
                {
                    // Es imar y aparece a la derecha
                    listaCuboAuto.Add(new Cubo(new Point(177, 0), obj2));
                }
            }
        }

        // Método de colisión del auto
        private void colisionCar()
        {
            #region Colisiona con un cubo
            while (true)
            {
                // Pausar el hilo para hacer el efecto del choque
                try
                {
                    Thread.Sleep(10);
                    if(Auto1.X == listaCuboAuto.ElementAt(0).X && listaCuboAuto.ElementAt(0).Y >= 390 && listaCuboAuto.ElementAt(0).Y <= 475)
                    {
                        // Colisión con el cubo pierde el juego
                        inflateCubo(listaCuboAuto.ElementAt(0));
                        Perdio();
                        // Cancelo los hilos
                        th_objAuto.Abort();
                    } else if (listaCuboAuto.ElementAt(0).Y > 475)
                    {
                        listaCuboAuto.RemoveAt(0);
                    }
                } catch (ArgumentOutOfRangeException)
                {

                }
                #endregion

                #region Colisiona con un circulo
                try
                {
                    if (Auto1.X == listaCirculoAuto.ElementAt(0).X && listaCirculoAuto.ElementAt(0).Y >= 390 && listaCirculoAuto.ElementAt(0).Y <= 475)
                    {
                        // Colisión con el cubo pierde el juego
                        inflateCirculo(listaCirculoAuto.ElementAt(0));
                        listaCirculoAuto.RemoveAt(0);
                        puntaje++;
                    }
                    else if (listaCirculoAuto.ElementAt(0).Y > 475)
                    {
                        inflateCirculo(listaCirculoAuto.ElementAt(0));
                        Perdio();
                        th_objAuto.Abort();
                    }
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }

        #endregion
        #endregion

        public void DibujarCarretera(PaintEventArgs e)
        {
            carretera = e.Graphics;
            carretera.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen pluma = new Pen(Color.White, 4);
            // Lineas del 1 al 3
            carretera.DrawLine(pluma, new Point(125, 0), new Point(125, Carretera.Height));
            label1.Text = puntaje.ToString();
        }

        private void Pista_Load(object sender, EventArgs e)
        {
            IniciaJuego();
        }

        private void Carretera_Paint(object sender, PaintEventArgs e)
        {
            DibujarCarretera(e);
            Auto1.Dibujar(e);

            try
            {
                foreach(Circulo item in listaCirculoAuto)
                {
                    item.Dibujar(e);
                }
                foreach(Cubo item in listaCuboAuto)
                {
                    item.Dibujar(e);
                }
            }
            catch (InvalidOperationException) { }
        }

        private void Pista_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    timerAnimationCar1.Start();
                    break;
                case Keys.R:
                    th_objAuto.Abort();
                    IniciaJuego();
                    break;
                case Keys.Q:
                    th_objAuto.Abort();
                    Application.Exit();
                    break;
            }
        }

        private void timerCar1_Tick(object sender, EventArgs e)
        {
            // Lanzar los objetos
            foreach (Circulo item in listaCirculoAuto)
            {
                item.Y += 6;
            }
            foreach (Cubo item in listaCuboAuto)
            {
                item.Y += 6;
            }
            Carretera.Refresh();
        }

        private void timerAnimationCar1_Tick(object sender, EventArgs e)
        {
            // Mover el auto a la izq o derecha
            switch (edo_ActualAuto1)
            {
                case EstadoActualAuto.izquierdo:
                    // Moverlo a la derecha
                    if(Auto1.X < posInicial_Auto1X + 125)
                    {
                        Auto1.X += 9;
                    } else
                    {
                        edo_ActualAuto1 = EstadoActualAuto.derecho;
                        Auto1.X = posInicial_Auto1X + 125;
                        timerAnimationCar1.Stop();
                    }
                    Carretera.Refresh();
                    break;
                case EstadoActualAuto.derecho:
                    // Moverlo a la izquiera
                    if(Auto1.X > posInicial_Auto1X)
                    {
                        Auto1.X -= 9;
                    } else
                    {
                        edo_ActualAuto1 = EstadoActualAuto.izquierdo;
                        Auto1.X = posInicial_Auto1X;
                        timerAnimationCar1.Stop();
                    }
                    Carretera.Refresh();
                    break;
            }
        }

        private void timerGenObjCar1_Tick(object sender, EventArgs e)
        {
            GenerarObjetoAuto1();
        }
    }
}
