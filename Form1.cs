
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RepairServiceTracker
{
    public partial class Form1 : Form
    {
        private BindingList<Repair> data =
            new BindingList<Repair>();

        private BindingSource source = new BindingSource();

        private Repair selectedRecord = null;
        private bool loading = false;
        private string imagePath = "";

        public Form1()
        {
            InitializeComponent();

            source.DataSource = data;
            dgvRepair.DataSource = source;

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnReset.Click += btnReset_Click;

            btnChooseImage.Click += btnChooseImage_Click;
            btnRemoveImage.Click += btnRemoveImage_Click;

            dgvRepair.SelectionChanged +=
                dgvRepair_SelectionChanged;

            txtSearch.TextChanged +=
                (s, e) => RefreshList();

            cboFilter.SelectedIndexChanged +=
                (s, e) => RefreshList();

            mnuExport.Click +=
                (s, e) => Ui.Export(dgvRepair);

            mnuExit.Click +=
                (s, e) => Close();

            ClearForm();
            RefreshList();
        }
        private bool ValidateInput(bool updating)
        {
            epCheck.Clear();
            bool valid = true;

            Action<Control, string> setError =
                (control, message) =>
                {
                    epCheck.SetError(control, message);
                    valid = false;
                };

            string code = txtId.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                setError(txtId, "Mã đơn không được để trống");
            }
            else if (data.Any(x =>
                string.Equals(x.Id, code,
                    StringComparison.OrdinalIgnoreCase)
                && (!updating || x != selectedRecord)))
            {
                setError(txtId, "Mã đơn đã tồn tại");
            }

            if (string.IsNullOrWhiteSpace(txtCustomer.Text))
            {
                setError(txtCustomer,
                    "Tên khách hàng không được để trống");
            }
            if (!Ui.Phone(txtPhone.Text.Trim()))
            {
                setError(txtPhone,
                    "Số điện thoại phải có 10 số, bắt đầu bằng 0");
            }
            if (cboDevice.SelectedIndex < 0)
            {
                setError(cboDevice, "Vui lòng chọn loại thiết bị");
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                setError(txtDescription,
                    "Mô tả lỗi không được để trống");
            }

            decimal price;

            if (!decimal.TryParse(
                txtCost.Text.Trim(), out price) || price < 0)
            {
                setError(txtCost,
                    "Chi phí phải là số không âm");
            }

            return valid;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(false))
                return;

            Repair repair = new Repair
            {
                Id = txtId.Text.Trim(),
                Customer = txtCustomer.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Device = cboDevice.Text,
                Description = txtDescription.Text.Trim(),
                Received = dtpReceived.Value.Date,
                Cost = decimal.Parse(txtCost.Text.Trim()),
                State = cboState.Text,
                ImagePath = imagePath
            };

            data.Add(repair);

            RefreshList();
            ClearForm();

            MessageBox.Show(
                "Thêm đơn sửa chữa thành công!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedRecord == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn đơn cần cập nhật!");
                return;
            }

            if (!ValidateInput(true))
                return;

            selectedRecord.Id = txtId.Text.Trim();
            selectedRecord.Customer = txtCustomer.Text.Trim();
            selectedRecord.Phone = txtPhone.Text.Trim();
            selectedRecord.Device = cboDevice.Text;
            selectedRecord.Description =
                txtDescription.Text.Trim();
            selectedRecord.Received = dtpReceived.Value.Date;
            selectedRecord.Cost =
                decimal.Parse(txtCost.Text.Trim());
            selectedRecord.State = cboState.Text;
            selectedRecord.ImagePath = imagePath;

            RefreshList();
            ClearForm();

            MessageBox.Show(
                "Cập nhật đơn sửa chữa thành công!");
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedRecord == null)
            {
                MessageBox.Show("Vui lòng chọn đơn cần xóa!");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa đơn " +
                    selectedRecord.Id + "?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            data.Remove(selectedRecord);

            RefreshList();
            ClearForm();

            MessageBox.Show("Xóa đơn thành công!");
        }

        private void btnChooseImage_Click(
            object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn ảnh thiết bị";

                dialog.Filter =
                    "Hình ảnh|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ShowImage(dialog.FileName);
                }
            }
        }
        private void ShowImage(string path)
        {
            
            Image oldImage = picDevice.Image;
            picDevice.Image = null;

            if (oldImage != null)
                oldImage.Dispose();

            if (string.IsNullOrWhiteSpace(path))
            {
                imagePath = "";
                return;
            }

            if (!File.Exists(path))
            {
                imagePath = "";
                return;
            }

            try
            {
                using (FileStream fs = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    using (Image image = Image.FromStream(fs))
                    {
                        picDevice.Image = new Bitmap(image);
                    }
                }

                imagePath = path;
                picDevice.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                imagePath = "";

                MessageBox.Show(
                    "Không thể đọc ảnh: " + ex.Message,
                    "Lỗi ảnh",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnRemoveImage_Click(
            object sender, EventArgs e)
        {
            ShowImage("");
        }

        private void dgvRepair_SelectionChanged(
            object sender, EventArgs e)
        {
            if (loading)
                return;

            if (dgvRepair.CurrentRow == null)
                return;

            Repair repair =
                dgvRepair.CurrentRow.DataBoundItem as Repair;

            if (repair == null)
                return;

            selectedRecord = repair;
            loading = true;

            try
            {
                txtId.Text = repair.Id;
                txtCustomer.Text = repair.Customer;
                txtPhone.Text = repair.Phone;
                cboDevice.SelectedItem = repair.Device;
                txtDescription.Text = repair.Description;
                dtpReceived.Value = repair.Received;
                txtCost.Text = repair.Cost.ToString("0");
                cboState.SelectedItem = repair.State;

                ShowImage(repair.ImagePath);
                epCheck.Clear();
            }
            finally
            {
                loading = false;
            }
        }
        private void RefreshList()
        {
            string keyword = txtSearch.Text.Trim();

            var result = data.Where(x =>
                (
                    keyword.Length == 0 ||
                    x.Id.IndexOf(
                        keyword,
                        StringComparison.OrdinalIgnoreCase) >= 0 ||
                    x.Customer.IndexOf(
                        keyword,
                        StringComparison.OrdinalIgnoreCase) >= 0 ||
                    x.Phone.Contains(keyword)
                )
                &&
                (
                    cboFilter.SelectedIndex <= 0 ||
                    x.State == cboFilter.Text
                )
            ).ToList();

            loading = true;

            try
            {
                source.DataSource =
                    new BindingList<Repair>(result);

                dgvRepair.ClearSelection();
                dgvRepair.CurrentCell = null;
                selectedRecord = null;
            }
            finally
            {
                loading = false;
            }

            decimal totalCost = result
                .Where(x =>
                    x.State != "Hoàn tất" &&
                    x.State != "Đã hủy")
                .Sum(x => x.Cost);

            lblStatus.Text =
                "Tổng đơn: " + data.Count +
                " | Đang hiển thị: " + result.Count +
                " | Chi phí dự kiến chưa hoàn tất: " +
                totalCost.ToString("N0") + " đ";
        }

        private void ClearForm()
        {
            loading = true;

            try
            {
                txtId.Clear();
                txtCustomer.Clear();
                txtPhone.Clear();
                txtDescription.Clear();

                cboDevice.SelectedIndex = 0;
                cboState.SelectedIndex = 0;

                dtpReceived.Value = DateTime.Today;
                txtCost.Text = "0";

                ShowImage("");
                selectedRecord = null;
                epCheck.Clear();

                dgvRepair.ClearSelection();
                dgvRepair.CurrentCell = null;
            }
            finally
            {
                loading = false;
            }
        }

        private void btnReset_Click(
            object sender, EventArgs e)
        {
            ClearForm();
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            Image oldImage = picDevice.Image;
            picDevice.Image = null;

            if (oldImage != null)
                oldImage.Dispose();

            base.OnFormClosed(e);
        }
    }
}
