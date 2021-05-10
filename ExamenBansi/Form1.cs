using apiexamen;
using Comun.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExamenBansi
{
    public partial class frmMain : Form
    {
        public int? ExamenSeleccionadoId;
        public string Servidor { get; set; }

        public frmMain()
        {
            InitializeComponent();
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarEntradas())
            {
                lblEstatus.ForeColor = System.Drawing.Color.Red;
                lblEstatus.Text = "Debe ingresar la informacion del examen";
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
                        lblEstatus.ForeColor = System.Drawing.Color.Green;
                        lblEstatus.Text = "Examen guardado exitosamente";
                        frmMain_Load(sender, e);
                    }
                }

                if (rbWebApi.Checked)
                {
                    if (await clsExamen.AgregarExamenApi(examen))
                    {
                        lblEstatus.ForeColor = System.Drawing.Color.Green;
                        lblEstatus.Text = "Examen guardado exitosamente";
                        frmMain_Load(sender, e);
                    }
                }                
            }
            catch (Exception ex)
            {
                lblEstatus.Text = ex.Message;
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

        private async void frmMain_Load(object sender, EventArgs e)
        {
            btnConfigurar.PerformClick();
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
                lblEstatus.Text = ex.Message;
            }            
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
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
            if (!ExamenSeleccionadoId.HasValue)
            {
                lblEstatus.Text = "Debe seleccionar un registro de la tabla para editar";
                return;
            }

            if (!ValidarEntradas())
            {
                lblEstatus.ForeColor = System.Drawing.Color.Red;
                lblEstatus.Text = "Debe ingresar la informacion del examen";
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
                        lblEstatus.ForeColor = System.Drawing.Color.Green;
                        lblEstatus.Text = "Examen actualizado exitosamente";
                        frmMain_Load(sender, e);
                    }
                }

                if (rbWebApi.Checked)
                {
                    if (await clsExamen.ActualizarExamenApi(examen))
                    {
                        lblEstatus.ForeColor = System.Drawing.Color.Green;
                        lblEstatus.Text = "Examen actualizado exitosamente";
                        frmMain_Load(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                lblEstatus.Text = ex.Message;
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!ExamenSeleccionadoId.HasValue)
            {
                lblEstatus.Text = "Debe seleccionar un registro de la tabla para eliminar";
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
                            lblEstatus.ForeColor = System.Drawing.Color.Green;
                            lblEstatus.Text = "Examen eliminado exitosamente";
                            Limpiar();
                            frmMain_Load(sender, e);
                        }
                    }

                    if (rbWebApi.Checked)
                    {
                        if (await clsExamen.EliminarExamenApi(ExamenSeleccionadoId.Value))
                        {
                            lblEstatus.ForeColor = System.Drawing.Color.Green;
                            lblEstatus.Text = "Examen eliminado exitosamente";
                            Limpiar();
                            frmMain_Load(sender, e);
                        }
                    }                    
                }
                else
                {
                    lblEstatus.Text = "Accion eliminar cancelada";
                }
            }
            catch (Exception ex)
            {
                lblEstatus.Text = ex.Message;
            }
        }

        private void btnConfigurar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtServidor.Text))
            {
                lblEstatus.Text = "Debe ingresar el nombre del servidor";
                return;
            }

            Servidor = txtServidor.Text.Trim();
        }
    }
}
