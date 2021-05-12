using apiexamen;
using Comun.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ExamenBansi
{
    public partial class frmMain : Form
    {
        public int? ExamenSeleccionadoId;
        bool EstaServidorConfigurado = false;
        public string Servidor { get; set; }

        public frmMain()
        {
            InitializeComponent();
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarServidorConfigurado())
            {
                return;
            }

            if (!ValidarEntradas())
            {
                MensajeDeSistema("Debe ingresar la informacion del examen", Color.Red);
                txtNombre.Focus();
                return;
            }

            ClsExamen clsExamen = new ClsExamen(Servidor);

            Examen examen = new Examen()
            {

                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text
            };

            try
            {
                if (rbProcedimiento.Checked)
                {
                    if (clsExamen.AgregarExamen(examen))
                    {
                        MensajeDeSistema("Examen guardado exitosamente", Color.Green);
                        Limpiar();
                    }
                }

                if (rbWebApi.Checked)
                {
                    if (await clsExamen.AgregarExamenApi(examen))
                    {
                        MensajeDeSistema("Examen guardado exitosamente", Color.Green);
                        Limpiar();
                    }
                }

                btnCargarDatos.PerformClick();
            }
            catch (Exception ex)
            {
                MensajeDeSistema(ex.Message, Color.Red);
            }
        }

        private bool ValidarEntradas()
        {
            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                return false;
            }

            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                return false;
            }

            lblEstatus.Text = string.Empty;
            return true;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
            MensajeDeSistema("Esperando accion del usuario", Color.RoyalBlue);

        }

        private void Limpiar()
        {
            ExamenSeleccionadoId = null;
            txtID.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            txtNombre.Focus();
        }

        private void dgvExamenes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ExamenSeleccionadoId = Convert.ToInt32(dgvExamenes.CurrentRow.Cells["IdExamen"].Value);
            var examen = dgvExamenes.CurrentRow.DataBoundItem as Examen;

            txtID.Text = examen.IdExamen.ToString();
            txtNombre.Text = examen.Nombre;
            txtDescripcion.Text = examen.Descripcion;
        }

        private async void btnEditar_Click(object sender, EventArgs e)
        {
            if (!ValidarServidorConfigurado())
            {
                return;
            }

            if (!ExamenSeleccionadoId.HasValue)
            {
                MensajeDeSistema("Debe seleccionar un registro de la tabla para editar", Color.Red);

                return;
            }

            if (!ValidarEntradas())
            {
                MensajeDeSistema("Debe ingresar la informacion del examen", Color.Red);

                return;
            }

            ClsExamen clsExamen = new ClsExamen(Servidor);

            Examen examen = new Examen()
            {
                IdExamen = ExamenSeleccionadoId.Value,
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text
            };

            try
            {
                if (rbProcedimiento.Checked)
                {
                    if (clsExamen.ActualizarExamen(examen))
                    {
                        MensajeDeSistema("Examen editado exitosamente", Color.Green);
                    }
                }

                if (rbWebApi.Checked)
                {
                    if (await clsExamen.ActualizarExamenApi(examen))
                    {
                        MensajeDeSistema("Examen editado exitosamente", Color.Green);

                    }
                }

                btnCargarDatos.PerformClick();
            }
            catch (Exception ex)
            {
                MensajeDeSistema(ex.Message, Color.Red);
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!ValidarServidorConfigurado())
            {
                return;
            }

            if (!ExamenSeleccionadoId.HasValue)
            {
                MensajeDeSistema("Debe seleccionar un registro de la tabla para eliminar", Color.Red);
                return;
            }

            ClsExamen clsExamen = new ClsExamen(Servidor);

            try
            {
                if (MessageBox.Show("Esta seguro de borrar el examen seleccionado?", "Eliminar examen", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rbProcedimiento.Checked)
                    {
                        if (clsExamen.EliminarExamen(ExamenSeleccionadoId.Value))
                        {
                            MensajeDeSistema("Examen eliminado exitosamente", Color.Green);
                            Limpiar();
                        }
                    }

                    if (rbWebApi.Checked)
                    {
                        if (await clsExamen.EliminarExamenApi(ExamenSeleccionadoId.Value))
                        {
                            MensajeDeSistema("Examen eliminado exitosamente", Color.Green);
                            Limpiar();
                        }
                    }
                    btnCargarDatos.PerformClick();
                }
                else
                {
                    MensajeDeSistema("Accion eliminar cancelada", Color.Green);
                }
            }
            catch (Exception ex)
            {
                MensajeDeSistema(ex.Message, Color.Red);
            }
        }

        private void btnConfigurar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtServidor.Text))
            {
                MensajeDeSistema("Servidor configurado...", Color.Red);
                return;
            }

            Servidor = txtServidor.Text.Trim();
            EstaServidorConfigurado = true;
            MensajeDeSistema("Servidor configurado...", Color.Green);
        }

        private async void btnCargarDatos_Click(object sender, EventArgs e)
        {
            ValidarServidorConfigurado();

            try
            {
                ClsExamen examen = new ClsExamen(Servidor);

                if (rbProcedimiento.Checked)
                {
                    var examenes = examen.ConsultarExamenes();
                    dgvExamenes.DataSource = examenes;
                }

                if (rbWebApi.Checked)
                {
                    var examenes = await examen.ConsultarExamenesApi();
                    dgvExamenes.DataSource = examenes;
                }

                Limpiar();
            }
            catch (Exception ex)
            {
                MensajeDeSistema(ex.Message, Color.Red);
            }
        }

        private void MensajeDeSistema(string mensaje, Color color)
        {
            lblEstatus.Text = mensaje;
            lblEstatus.ForeColor = color;
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            if (!ValidarServidorConfigurado())
            {
                return;
            }

            if (!ValidarEntradas())
            {
                MensajeDeSistema("Debe ingresar la informacion del examen", Color.Red);

                return;
            }

            ClsExamen clsExamen = new ClsExamen(Servidor);

            try
            {
                if (rbProcedimiento.Checked)
                {
                    var examen = clsExamen.ConsultarExamen(txtNombre.Text, txtDescripcion.Text);

                    if (examen.Count > 0)
                    {
                        dgvExamenes.DataSource = examen.ToList();
                    }
                    else
                    {
                        MensajeDeSistema("No se encontraron registros con la informacion proporcionada", Color.Orange);
                        dgvExamenes.DataSource = null;
                    }
                }

                if (rbWebApi.Checked)
                {
                    var examenes = await clsExamen.ConsultarExamenApi(txtNombre.Text, txtDescripcion.Text);

                    if (examenes.Count > 0)
                    {
                        dgvExamenes.DataSource = examenes;
                    }
                    else
                    {
                        MensajeDeSistema("No se encontraron registros con la informacion proporcionada", Color.Orange);
                        dgvExamenes.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MensajeDeSistema(ex.Message, Color.Red);
            }
        }

        private bool ValidarServidorConfigurado()
        {
            if (!EstaServidorConfigurado)
            {
                MensajeDeSistema("Debe configurar el servidor: Escriba el nombre y presione Configurar", Color.Red);
                return false;
            }

            return true;
        }
    }
}
