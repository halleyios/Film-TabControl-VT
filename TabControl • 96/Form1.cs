using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TabControl___96
{
    public partial class Form1 : Form
    {
        string baglanti = "Server=localhost;Database=film_arsiv;Uid=root;Pwd='';";
        string yeniAd;
        public Form1()
        {
            InitializeComponent();
        } 
        
        void DgwDoldur()
        {
                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    baglan.Open();
                    string sorgu = "SELECT * FROM filmler;";

                    MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    dgwTumFilmler.DataSource = dt;

                }
        }
        void CmbDoldur()
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "SELECT DISTINCT tur FROM filmler;";

                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                cmbTur.DataSource = dt;
                cmbTur.DisplayMember = "tur";
                cmbTur.ValueMember = "tur";

                cmbYeniTur.DataSource = dt;
                cmbYeniTur.DisplayMember = "tur";
                cmbYeniTur.ValueMember = "tur";

            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string klasorYolu = @"poster";
            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
            }

            DgwDoldur();
            CmbDoldur();
        }
        private void dgwTumFilmler_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwTumFilmler.SelectedCells.Count > 0)
            {
                txtAd.Text = dgwTumFilmler.SelectedRows[0].Cells["film_ad"].Value.ToString();
                txtYonetmen.Text = dgwTumFilmler.SelectedRows[0].Cells["yonetmen"].Value.ToString();
                txtYil.Text = dgwTumFilmler.SelectedRows[0].Cells["yil"].Value.ToString();
                cmbTur.SelectedValue = dgwTumFilmler.SelectedRows[0].Cells["tur"].Value.ToString();
                txtSure.Text = dgwTumFilmler.SelectedRows[0].Cells["sure"].Value.ToString();
                // txtPoster.Text = dgwTumFilmler.SelectedRows[0].Cells["poster"].Value.ToString();
                txtPuan.Text = dgwTumFilmler.SelectedRows[0].Cells["imdb_puan"].Value.ToString();
                cbOdul.Checked = Convert.ToBoolean(dgwTumFilmler.SelectedRows[0].Cells["film_odul"].Value);

                lblAd.Text = dgwTumFilmler.SelectedRows[0].Cells["film_ad"].Value.ToString();
                lblYonetmen.Text = dgwTumFilmler.SelectedRows[0].Cells["yonetmen"].Value.ToString();
                lblYil.Text = dgwTumFilmler.SelectedRows[0].Cells["yil"].Value.ToString();
                lblTur.Text = dgwTumFilmler.SelectedRows[0].Cells["tur"].Value.ToString();
                lblSure.Text = dgwTumFilmler.SelectedRows[0].Cells["sure"].Value.ToString();
                lblPuan.Text = dgwTumFilmler.SelectedRows[0].Cells["imdb_puan"].Value.ToString();
                lblOdul.Text = dgwTumFilmler.SelectedRows[0].Cells["film_odul"].Value.ToString();

                string dosyaYolu = Path.Combine(Environment.CurrentDirectory, "poster", dgwTumFilmler.SelectedRows[0].Cells["poster"].Value.ToString());

                pbResimGuncelle.ImageLocation = null;
                pbResimListele.ImageLocation = null;
                if (File.Exists(dosyaYolu))
                {
                    pbResimListele.ImageLocation = dosyaYolu;
                    pbResimListele.SizeMode = PictureBoxSizeMode.StretchImage;

                    pbResimGuncelle.ImageLocation = dosyaYolu;
                    pbResimGuncelle.SizeMode = PictureBoxSizeMode.StretchImage;
                }

            } 

        }

        private void pbResimListele_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "poster", yeniAd);

            File.Copy(kaynakDosya, hedefDosya);

            pbResimGuncelle.Image = null;
            if (File.Exists(hedefDosya))
            {
                pbResimGuncelle.Image = Image.FromFile(hedefDosya);
                pbResimGuncelle.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            DataGridViewRow dr = dgwTumFilmler.SelectedRows[0];

            int id = Convert.ToInt32(dr.Cells[0].Value);

            string posterYol = Path.Combine(Environment.CurrentDirectory, "poster", dgwTumFilmler.SelectedRows[0].Cells["poster"].Value.ToString());


            DialogResult cevap = MessageBox.Show("Filmi silmek istediğinizden emin misiniz?",
                                                 "Filmi sil", MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);


            if (cevap == DialogResult.Yes)
            {

                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    int film_id = Convert.ToInt32(dgwTumFilmler.SelectedRows[0].Cells["film_id"].Value);
                    baglan.Open();
                    string sorgu = "DELETE FROM filmler WHERE film_id=@film_id;";
                    MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                    cmd.Parameters.AddWithValue("@film_id", film_id);
                    cmd.ExecuteNonQuery();


                    if (File.Exists(posterYol))
                    {

                        File.Delete(posterYol);
                    }
                    DgwDoldur();

                }
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "UPDATE filmler SET film_ad=@film_ad, yonetmen = @yonetmen, yil = @yil, tur=@tur, sure= @sure, poster = @poster, imdb_puan = @imdb_puan, film_odul = @film_odul WHERE film_id = @film_id";

                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                cmd.Parameters.AddWithValue("@film_ad", txtAd.Text);
                cmd.Parameters.AddWithValue("@yonetmen", txtYonetmen.Text);
                cmd.Parameters.AddWithValue("@yil", txtYil.Text);
                cmd.Parameters.AddWithValue("@tur", cmbTur.SelectedValue);
                cmd.Parameters.AddWithValue("@sure", txtSure.Text);
                cmd.Parameters.AddWithValue("@imdb_puan", Convert.ToDouble(txtPuan.Text));
                cmd.Parameters.AddWithValue("@film_odul", cbOdul.Checked);
                int film_id = Convert.ToInt32(dgwTumFilmler.SelectedRows[0].Cells["film_id"].Value);
                cmd.Parameters.AddWithValue("@film_id", film_id);
                cmd.Parameters.AddWithValue("@poster", yeniAd);

                cmd.ExecuteNonQuery();
                DgwDoldur();

            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "INSERT INTO filmler VALUES(NULL,@film_ad,@yonetmen,@yil,@tur,@sure,@poster,@imdb_puan,@film_odul);";
                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                cmd.Parameters.AddWithValue("@film_ad", txtYeniAd.Text);
                cmd.Parameters.AddWithValue("@yonetmen", txtYeniYonetmen.Text);
                cmd.Parameters.AddWithValue("@yil", txtYeniYil.Text);
                cmd.Parameters.AddWithValue("@tur", cmbYeniTur.SelectedValue);
                cmd.Parameters.AddWithValue("@sure", txtYeniSure.Text);
                cmd.Parameters.AddWithValue("@poster", yeniAd);
                cmd.Parameters.AddWithValue("@imdb_puan", txtYeniPuan.Text);
                cmd.Parameters.AddWithValue("@film_odul", cbYeniOdul.Checked);

                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Kayıt Eklendi");
                }
                DgwDoldur();
            }
        }

        private void pbResimGuncelle_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "poster", yeniAd);

            File.Copy(kaynakDosya, hedefDosya);

            pbResimGuncelle.Image = null;
            if (File.Exists(hedefDosya))
            {
                pbResimGuncelle.Image = Image.FromFile(hedefDosya);
                pbResimGuncelle.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void pbYeniResim_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "poster", yeniAd);

            File.Copy(kaynakDosya, hedefDosya);

            pbYeniResim.ImageLocation = null;
            if (File.Exists(hedefDosya))
            {
                pbYeniResim.ImageLocation = hedefDosya;
                pbYeniResim.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
