using System;
using System.Drawing;
using System.Windows.Forms;

namespace envanter
{
    public partial class AddProductForm : Form
    {
        private TextBox txtName;
        private NumericUpDown numQuantity;
        private PictureBox pictureBox;
        private Button btnSelectImage;
        private Button btnSave;
        private Button btnCancel;
        private Image selectedImage;

        public Product Product { get; private set; }

        public AddProductForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Yeni Ürün Ekle";
            this.Size = new Size(400, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Ürün adı
            Label lblName = new Label();
            lblName.Text = "Ürün Adı:";
            lblName.Location = new Point(20, 20);
            lblName.AutoSize = true;
            this.Controls.Add(lblName);

            txtName = new TextBox();
            txtName.Location = new Point(20, 40);
            txtName.Size = new Size(340, 25);
            this.Controls.Add(txtName);

            // Ürün miktarı
            Label lblQuantity = new Label();
            lblQuantity.Text = "Miktar:";
            lblQuantity.Location = new Point(20, 70);
            lblQuantity.AutoSize = true;
            this.Controls.Add(lblQuantity);

            numQuantity = new NumericUpDown();
            numQuantity.Location = new Point(20, 90);
            numQuantity.Size = new Size(340, 25);
            numQuantity.Minimum = 0;
            numQuantity.Maximum = 999999;
            this.Controls.Add(numQuantity);

            // Fotoğraf seçme
            btnSelectImage = new Button();
            btnSelectImage.Text = "Fotoğraf Seç";
            btnSelectImage.Location = new Point(20, 120);
            btnSelectImage.Size = new Size(340, 30);
            btnSelectImage.Click += BtnSelectImage_Click;
            this.Controls.Add(btnSelectImage);

            pictureBox = new PictureBox();
            pictureBox.Location = new Point(20, 160);
            pictureBox.Size = new Size(340, 200);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pictureBox);

            // Kaydet ve İptal butonları
            btnSave = new Button();
            btnSave.Text = "Kaydet";
            btnSave.Location = new Point(20, 380);
            btnSave.Size = new Size(165, 30);
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button();
            btnCancel.Text = "İptal";
            btnCancel.Location = new Point(195, 380);
            btnCancel.Size = new Size(165, 30);
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);
        }

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Title = "Ürün Fotoğrafı Seç";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImage = Image.FromFile(openFileDialog.FileName);
                    pictureBox.Image = selectedImage;
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Lütfen ürün adını giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (selectedImage == null)
            {
                MessageBox.Show("Lütfen bir fotoğraf seçiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Product = new Product
            {
                Name = txtName.Text,
                Quantity = (int)numQuantity.Value,
                Image = selectedImage
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
} 