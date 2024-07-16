using System.Diagnostics;

namespace _5834163DemoIntroAsync
{
    public partial class Form1 : Form
    {
        HttpClient httpClient = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        //async void debe ser evitado, EXCEPTO en eventos!
        private async void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Has clikeado el bot�n");

            pictureBox1.Visible = true;

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes\resultado-secuencial");
            var destinoBaseParalelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecucion(destinoBaseParalelo, destinoBaseSecuencial);

            Console.WriteLine("inicio");
            List<Imagen> imagenes = ObtenerImagenes();

            //Parte secuencial
            var sw = new Stopwatch();
            sw.Start();

            foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }
            Console.WriteLine("Secuencial - duraci�n en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Reset();
            sw.Start();

            var tareasEnumerables = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBaseParalelo, imagen);
            });

            await Task.WhenAll(tareasEnumerables);

            Console.WriteLine("Paralelo-duraci�n en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Stop();

            #region proceso lento
            ////proceso lento
            //var tareas = new List<Task>()
            //{
            //    RealizarProcesamientoLargoA(),
            //    RealizarProcesamientoLargoB(),
            //    RealizarProcesamientoLargoC()
            //};
            //await Task.WhenAll(tareas);

            //sw.Stop();

            //var duraci�n = $"El programa se ejecut� en {sw.ElapsedMilliseconds / 1000.0} segundos";
            //Console.WriteLine(duraci�n);
            #endregion


            pictureBox1.Visible = false;

            #region Inicio del Video

            //proceso lento
            //  Thread.Sleep(5000); //sincrono

            // await Task.Delay(5000); //asincrono

            //var nombre= await ProcesamientoLargo();

            //MessageBox.Show($"Saludos, {nombre}");
            #endregion

        }

        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;
            using (var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }

            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);
        }

        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();

            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Bandera {i}.png",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/34/Flag_of_El_Salvador.svg/150px-Flag_of_El_Salvador.svg.png"
                    });

                imagenes.Add(
                       new Imagen()
                       {
                           Nombre = $"�rbol Nacional {i}.jpg",
                           URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d0/Cerejeira_pg.jpg/280px-Cerejeira_pg.jpg"
                       });

                imagenes.Add(
                new Imagen()
                {
                    Nombre = $"Presidente de El Salvador {i}.jpg",
                    URL = "https://upload.wikimedia.org/wikipedia/commons/e/ef/Presidente_Bukele_%28cropped%29.jpg"

                });
            }
            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecucion(string destinoBaseParalelo, string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }

            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoBaseParalelo);
        }


        #region procesosA,B,C
        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso A finalizado");
        }

        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso B finalizado");
        }

        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso C finalizado");
        }
        #endregion

        #region Inicio
        //private async Task<string> ProcesamientoLargo()
        //{
        //    await Task.Delay(3000);
        //    return "Felipe";
        //}

        //private async Task ProcesamientoLargo()
        //{
        //    await Task.Delay(5000);
        //}
        #endregion
    }
}

