    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
using De02.Model;

    namespace De02
    {
        public partial class Form1 : Form
        {
        private Model1 dbContext;
        public Form1()
            {
                InitializeComponent();
            dbContext = new Model1();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadLoaiSPToComboBox();
        }
        private void LoadData()
        {
            try
            {
                
                var data = from sp in dbContext.Sanpham
                           join loai in dbContext.LoaiSP on sp.MaLoai equals loai.MaLoai
                           select new
                           {
                               sp.MaSP,
                               sp.TenSP,
                               sp.NgayNhap,
                               LoaiSP = loai.TenLoai
                           };

                dataGridView1.AutoGenerateColumns = false;

                
                dataGridView1.Columns.Clear();

                
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "MaSP",
                    HeaderText = "Mã Sản Phẩm",
                    Name = "MaSP"
                });

                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "TenSP",
                    HeaderText = "Tên Sản Phẩm",
                    Name = "TenSP"
                });

                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "NgayNhap",
                    HeaderText = "Ngày Nhập",
                    Name = "NgayNhap"
                });

                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "LoaiSP",
                    HeaderText = "Loại Sản Phẩm",
                    Name = "LoaiSP"
                });

                
                dataGridView1.DataSource = data.ToList();
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Có lỗi khi tải dữ liệu: " + ex.Message);
            }

        }
        private void LoadLoaiSPToComboBox()
        {
            try
            {
                // Lấy danh sách tên loại sản phẩm từ bảng LoaiSP
                var loaiSPList = dbContext.LoaiSP
                                          .Select(loai => loai.TenLoai)
                                          .ToList();

                // Gán dữ liệu vào ComboBox
                cmbLoai.DataSource = loaiSPList;
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu có
                MessageBox.Show("Có lỗi khi tải dữ liệu vào ComboBox: " + ex.Message);
            }
        }
        private void LoadDataToControls()
        {
            try
            {
                // Kiểm tra xem có hàng nào được chọn không
                if (dataGridView1.CurrentRow != null)
                {
                    // Lấy dữ liệu từ hàng hiện tại
                    DataGridViewRow selectedRow = dataGridView1.CurrentRow;

                    // Gán giá trị vào các TextBox
                    txtMaSP.Text = selectedRow.Cells["MaSP"].Value?.ToString();
                    txtTenSP.Text = selectedRow.Cells["TenSP"].Value?.ToString();
                    date.Value = Convert.ToDateTime(selectedRow.Cells["NgayNhap"].Value);
                    cmbLoai.Text = selectedRow.Cells["LoaiSP"].Value?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi khi tải dữ liệu vào các điều khiển: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            LoadDataToControls();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtMaSP.Text) || string.IsNullOrWhiteSpace(txtTenSP.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo đối tượng Sanpham mới
                var newSanPham = new Model.Sanpham
                {
                    MaSP = txtMaSP.Text.Trim(),
                    TenSP = txtTenSP.Text.Trim(),
                    NgayNhap = date.Value,
                    MaLoai = dbContext.LoaiSP.FirstOrDefault(loai => loai.TenLoai == cmbLoai.Text)?.MaLoai
                };

                // Kiểm tra mã loại
                if (newSanPham.MaLoai == null)
                {
                    MessageBox.Show("Loại sản phẩm không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                dbContext.Sanpham.Add(newSanPham);
                dbContext.SaveChanges();

                // Tải lại dữ liệu lên DataGridView
                LoadData();

                // Xóa các điều khiển
                ClearControls();

                MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearControls()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            date.Value = DateTime.Now;
            cmbLoai.SelectedIndex = -1; // Bỏ chọn trong ComboBox
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(txtMaSP.Text) || string.IsNullOrWhiteSpace(txtTenSP.Text))
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần sửa và nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tìm sản phẩm trong cơ sở dữ liệu dựa trên MaSP
                var existingSanPham = dbContext.Sanpham.FirstOrDefault(sp => sp.MaSP == txtMaSP.Text.Trim());

                if (existingSanPham == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm với mã này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cập nhật thông tin sản phẩm
                existingSanPham.TenSP = txtTenSP.Text.Trim();
                existingSanPham.NgayNhap = date.Value;
                existingSanPham.MaLoai = dbContext.LoaiSP.FirstOrDefault(loai => loai.TenLoai == cmbLoai.Text)?.MaLoai;

                // Kiểm tra mã loại sản phẩm
                if (existingSanPham.MaLoai == null)
                {
                    MessageBox.Show("Loại sản phẩm không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                dbContext.SaveChanges();

                // Tải lại dữ liệu lên DataGridView
                LoadData();

                // Xóa các điều khiển
                ClearControls();

                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu người dùng chưa chọn sản phẩm để xóa
                if (string.IsNullOrWhiteSpace(txtMaSP.Text))
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hiển thị hộp thoại xác nhận trước khi xóa
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này không?",
                                                    "Xác nhận xóa",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    // Tìm sản phẩm cần xóa dựa trên MaSP
                    var sanPhamToDelete = dbContext.Sanpham.FirstOrDefault(sp => sp.MaSP == txtMaSP.Text.Trim());

                    if (sanPhamToDelete == null)
                    {
                        MessageBox.Show("Không tìm thấy sản phẩm để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Xóa sản phẩm
                    dbContext.Sanpham.Remove(sanPhamToDelete);
                    dbContext.SaveChanges();

                    // Làm mới dữ liệu hiển thị trong DataGridView
                    LoadData();

                    // Xóa các điều khiển
                    ClearControls();

                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng?",
                                       "Xác nhận thoát",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                // Đóng ứng dụng
                this.Close();
            }

        }
    }
}
