using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace De02
{
    public partial class frmQLSP : Form
    {

        private readonly SPService SPService = new SPService();
        private readonly LoaiService LoaiService = new LoaiService();
        public frmQLSP()
        {
            InitializeComponent();
        }

        private void FillCategoryCombobox(List<LoaiSP> listCategories)
        {
            listCategories.Insert(0, new LoaiSP());
            this.cmbLoaiSP.DataSource = listCategories;
            this.cmbLoaiSP.DisplayMember = "TenLoai";
            this.cmbLoaiSP.ValueMember = "MaLoai";
        }

        private void BindGrid(List<SanPham> listSP)
        {
            dgvSanPham.Rows.Clear();
            foreach (var item in listSP)
            {
                int index = dgvSanPham.Rows.Add();
                dgvSanPham.Rows[index].Cells[0].Value = item.MaSP;
                dgvSanPham.Rows[index].Cells[1].Value = item.TenSP;
                dgvSanPham.Rows[index].Cells[2].Value = item.NgayNhap;
                if (item.LoaiSP != null)
                    dgvSanPham.Rows[index].Cells[3].Value = item.LoaiSP.TenLoai;
            }
        }
        private void frmQLSP_Load(object sender, EventArgs e)
        {
            var listLoai = LoaiService.GetAll();
            var listSP = SPService.GetAll();
            FillCategoryCombobox(listLoai);
            BindGrid(listSP);
            btnLuu.Enabled = false;
            btnKLuu.Enabled = false;
        }
        private bool ValidateInput()
        {
            bool valid = true;
            err.Clear();
            if (string.IsNullOrWhiteSpace(txtMaSP.Text) ||
                string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return false;
            }

            return valid;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
                err.Clear();
                if (!ValidateInput()) return;

                // Check if the product already exists
                bool exists = dgvSanPham.Rows.Cast<DataGridViewRow>()
                                             .Any(r => r.Cells[0].Value.ToString() == txtMaSP.Text);

                if (!exists)
                {
                    try
                    {
                        // Create new product object
                        SanPham newProduct = new SanPham
                        {
                            MaSP = txtMaSP.Text,
                            TenSP = txtTenSP.Text,
                            NgayNhap = dtNgayNhap.Value,
                            MaLoai = cmbLoaiSP.SelectedValue.ToString(),
                        };

                        // Add new product to the database
                        SPService.AddProduct(newProduct);
                        UpdateProductList();

                        MessageBox.Show("Thêm mới sản phẩm thành công!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Mã sản phẩm đã tồn tại.");
                }
                ResetInputFields();
            

        }
        private void UpdateProductList()
        {
            try
            {
                var ListLoai = LoaiService.GetAll(); // Get faculties
                var listSP = SPService.GetAll(); // Get students
                FillCategoryCombobox(ListLoai);
                BindGrid(listSP);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ResetInputFields()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            cmbLoaiSP.SelectedIndex = -1;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {

            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?",
                                         "Xác nhận thoát",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            err.Clear();
            if (!ValidateInput()) return;

            try
            {
                var sanpham = SPService.GetProductById(txtMaSP.Text); // Get student by ID
                if (sanpham != null)
                {
                    // Update student info
                    sanpham.TenSP = txtTenSP.Text;
                    sanpham.NgayNhap = dtNgayNhap.Value;
                    sanpham.MaLoai = cmbLoaiSP.SelectedValue.ToString();


                    SPService.UpdateProduct(sanpham); // Update student in database

                    UpdateProductList();

                    MessageBox.Show("Sửa dữ liệu thành công!");
                }
                else
                {
                    MessageBox.Show("San pham không tồn tại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ResetInputFields();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSP.Text))
            {
                MessageBox.Show("Vui lòng chọn san pham cần xóa.");
                return;
            }
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa san pham này không?",
                            "Xác nhận xóa",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    var sanpham = SPService.GetProductById(txtMaSP.Text);
                    if (sanpham != null)
                    {
                        SPService.DeleteProduct(txtMaSP.Text); // Delete student from database


                        UpdateProductList();
                        MessageBox.Show("Xóa dữ liệu thành công!");
                    }
                    else
                    {
                        MessageBox.Show("San pham không tồn tại.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            ResetInputFields();
        }

        private void dgvSanPham_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Load data from DataGridView to input fields
                DataGridViewRow row = dgvSanPham.Rows[e.RowIndex];
                txtMaSP.Text = row.Cells[0].Value.ToString();
                txtTenSP.Text = row.Cells[1].Value.ToString();
                cmbLoaiSP.Text = row.Cells[3].Value.ToString(); 
                dtNgayNhap.Value = DateTime.Parse(row.Cells[2].Value.ToString());


            }
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                row.Visible = true;
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFind.Text))
            {
                MessageBox.Show("Vui lòng nhập mã hoặc tên sinh viên cần tìm.");
                return;
            }
            string searchValue = txtFind.Text.ToLower();

            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                bool found = row.Cells[0].Value.ToString().ToLower().Contains(searchValue) ||
                             row.Cells[1].Value.ToString().ToLower().Contains(searchValue);

                row.Visible = found;
            }
        }
    }
}
