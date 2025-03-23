using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace envanter
{
    public partial class Form1 : Form
    {
        private List<Product> products = new List<Product>();
        private FlowLayoutPanel productPanel;
        private Button btnAddProduct;
        private Button btnDeleteProduct;
        private TextBox txtSearch;
        private Button btnSearch;
        private string dataFilePath = Path.Combine(Application.StartupPath, "products.json");

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadProducts();
        }

        private void InitializeCustomComponents()
        {
            // Form boyutunu ayarla
            this.Size = new Size(800, 600);
            this.Text = "Envanter Takip Sistemi";

            // Arama butonu ve textbox
            txtSearch = new TextBox();
            txtSearch.Location = new Point(500, 20);
            txtSearch.Size = new Size(200, 25);
            this.Controls.Add(txtSearch);

            btnSearch = new Button();
            btnSearch.Text = "Ara";
            btnSearch.Location = new Point(710, 20);
            btnSearch.Size = new Size(70, 25);
            btnSearch.Click += BtnSearch_Click;
            this.Controls.Add(btnSearch);

            // Ürün ekle ve sil butonları
            btnAddProduct = new Button();
            btnAddProduct.Text = "Ürün Ekle";
            btnAddProduct.Location = new Point(20, 20);
            btnAddProduct.Size = new Size(100, 30);
            btnAddProduct.Click += BtnAddProduct_Click;
            this.Controls.Add(btnAddProduct);

            btnDeleteProduct = new Button();
            btnDeleteProduct.Text = "Ürün Sil";
            btnDeleteProduct.Location = new Point(130, 20);
            btnDeleteProduct.Size = new Size(100, 30);
            btnDeleteProduct.Click += BtnDeleteProduct_Click;
            this.Controls.Add(btnDeleteProduct);

            // Ürün listesi paneli
            productPanel = new FlowLayoutPanel();
            productPanel.Location = new Point(20, 70);
            productPanel.Size = new Size(740, 470);
            productPanel.AutoScroll = true;
            this.Controls.Add(productPanel);
        }

        private void LoadProducts()
        {
            try
            {
                if (File.Exists(dataFilePath))
                {
                    string jsonData = File.ReadAllText(dataFilePath);
                    var productDataList = JsonSerializer.Deserialize<List<ProductData>>(jsonData);
                    
                    foreach (var productData in productDataList)
                    {
                        Product product = new Product
                        {
                            Name = productData.Name,
                            Quantity = productData.Quantity,
                            Image = Image.FromFile(productData.ImagePath)
                        };
                        products.Add(product);
                        AddProductToPanel(product);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veriler yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveProducts()
        {
            try
            {
                var productDataList = new List<ProductData>();
                foreach (var product in products)
                {
                    string imagePath = Path.Combine(Application.StartupPath, "Images", $"{Guid.NewGuid()}.png");
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                    product.Image.Save(imagePath);
                    
                    productDataList.Add(new ProductData
                    {
                        Name = product.Name,
                        Quantity = product.Quantity,
                        ImagePath = imagePath
                    });
                }

                string jsonData = JsonSerializer.Serialize(productDataList);
                File.WriteAllText(dataFilePath, jsonData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veriler kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            SaveProducts();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            foreach (Control control in productPanel.Controls)
            {
                if (control is Panel productControl)
                {
                    Label nameLabel = (Label)productControl.Controls["lblName"];
                    bool isVisible = nameLabel.Text.ToLower().Contains(searchText);
                    productControl.Visible = isVisible;
                }
            }
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            using (var addForm = new AddProductForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    Product newProduct = addForm.Product;
                    products.Add(newProduct);
                    AddProductToPanel(newProduct);
                    ShowSuccessMessage();
                }
            }
        }

        private void BtnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (productPanel.Controls.Count == 0)
            {
                MessageBox.Show("Silinecek ürün bulunmamaktadır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("Seçili ürünü silmek istediğinizden emin misiniz?", "Onay", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                foreach (Control control in productPanel.Controls)
                {
                    if (control is Panel productControl)
                    {
                        if (productControl.Tag is Product product)
                        {
                            products.Remove(product);
                            productControl.Dispose();
                            break;
                        }
                    }
                }
            }
        }

        private void AddProductToPanel(Product product)
        {
            Panel productPanel = new Panel();
            productPanel.Size = new Size(200, 250);
            productPanel.BorderStyle = BorderStyle.FixedSingle;
            productPanel.Tag = product; // Ürün bilgisini panel'e bağla

            PictureBox pictureBox = new PictureBox();
            pictureBox.Size = new Size(180, 180);
            pictureBox.Location = new Point(10, 10);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Image = product.Image;
            pictureBox.Name = "picProduct";

            Label nameLabel = new Label();
            nameLabel.Text = product.Name;
            nameLabel.Location = new Point(10, 200);
            nameLabel.Size = new Size(180, 20);
            nameLabel.Name = "lblName";

            Label quantityLabel = new Label();
            quantityLabel.Text = $"Miktar: {product.Quantity}";
            quantityLabel.Location = new Point(10, 220);
            quantityLabel.Size = new Size(180, 20);

            productPanel.Controls.Add(pictureBox);
            productPanel.Controls.Add(nameLabel);
            productPanel.Controls.Add(quantityLabel);

            this.productPanel.Controls.Add(productPanel);
        }

        private void ShowSuccessMessage()
        {
            Label successLabel = new Label();
            successLabel.Text = "Ürün başarıyla eklendi!";
            successLabel.ForeColor = Color.Green;
            successLabel.Font = new Font(Font.FontFamily, 12, FontStyle.Bold);
            successLabel.AutoSize = true;
            successLabel.Location = new Point(300, 20);
            this.Controls.Add(successLabel);

            System.Timers.Timer timer = new System.Timers.Timer(3000); // Değiştirildi
            timer.Elapsed += (s, e) =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    successLabel.Visible = false;
                });
                timer.Stop();
            };
            timer.Start();
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Image Image { get; set; }
    }

    public class ProductData
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }
    }
}
