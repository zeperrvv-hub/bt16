
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace LabEquipmentCheckout
{
    public partial class Form1 : Form
    {
        private BindingList<Checkout> data =
            new BindingList<Checkout>();

        private BindingSource source = new BindingSource();

        private Checkout selectedRecord = null;
        private bool loading = false;

        public Form1()
        {
            InitializeComponent();

            source.DataSource = data;
            dgvCheckout.DataSource = source;

            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnMarkReturned.Click += btnMarkReturned_Click;
            btnReset.Click += btnReset_Click;

            dgvCheckout.SelectionChanged +=  dgvCheckout_SelectionChanged;
            txtSearch.TextChanged += (s, e) => RefreshList();
            cboFilter.SelectedIndexChanged += (s, e) => RefreshList();
            cboState.SelectedIndexChanged += cboState_SelectedIndexChanged;

            mnuExport.Click += (s, e) =>
                Ui.Export(dgvCheckout);

            mnuExit.Click += (s, e) => Close();

            ClearForm();
            RefreshList();
        }

        private bool ValidateInput(bool updating)
        {
            epCheck.Clear();
            bool valid = true;

            Action<Control, string> setError = (control, message) =>
            {
                epCheck.SetError(control, message);
                valid = false;
            };

            string code = txtId.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                setError(txtId, "Mã phiếu không được để trống");
            }
            else if (data.Any(x =>
                string.Equals(x.Id, code,
                    StringComparison.OrdinalIgnoreCase)
                && (!updating || x != selectedRecord)))
            {
                setError(txtId, "Mã phiếu đã tồn tại");
            }

            if (string.IsNullOrWhiteSpace(txtStudentId.Text))
            {
                setError(txtStudentId,
                    "Mã sinh viên không được để trống");
            }

            if (string.IsNullOrWhiteSpace(txtStudent.Text))
            {
                setError(txtStudent,
                    "Họ tên sinh viên không được để trống");
            }

            if (cboEquipment.SelectedIndex < 0)
            {
                setError(cboEquipment, "Vui lòng chọn thiết bị");
            }

            int qty;

            if (!int.TryParse(txtQuantity.Text.Trim(), out qty)
                || qty <= 0)
            {
                setError(txtQuantity,
                    "Số lượng phải là số nguyên lớn hơn 0");
            }

            if (dtpDue.Value.Date < dtpBorrow.Value.Date)
            {
                setError(dtpDue,
                    "Hạn trả không được trước ngày mượn");
            }

            if (cboState.Text == "Đã trả")
            {
                if (!dtpReturned.Checked)
                {
                    setError(dtpReturned,
                        "Vui lòng chọn ngày trả thực tế");
                }
                else if (dtpReturned.Value.Date <
                         dtpBorrow.Value.Date)
                {
                    setError(dtpReturned,
                        "Ngày trả không được trước ngày mượn");
                }
            }

            return valid;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(false))
                return;

            Checkout checkout = new Checkout
            {
                Id = txtId.Text.Trim(),
                StudentId = txtStudentId.Text.Trim(),
                Student = txtStudent.Text.Trim(),
                Equipment = cboEquipment.Text,
                Quantity = int.Parse(txtQuantity.Text.Trim()),
                Borrow = dtpBorrow.Value.Date,
                Due = dtpDue.Value.Date,
                Returned = cboState.Text == "Đã trả"
                    ? (DateTime?)dtpReturned.Value.Date
                    : null
            };

            data.Add(checkout);

            MessageBox.Show(
                "Thêm phiếu mượn thành công!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            RefreshList();
            ClearForm();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedRecord == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu cần cập nhật!");
                return;
            }

            if (!ValidateInput(true))
                return;

            selectedRecord.Id = txtId.Text.Trim();
            selectedRecord.StudentId = txtStudentId.Text.Trim();
            selectedRecord.Student = txtStudent.Text.Trim();
            selectedRecord.Equipment = cboEquipment.Text;
            selectedRecord.Quantity =
                int.Parse(txtQuantity.Text.Trim());

            selectedRecord.Borrow = dtpBorrow.Value.Date;
            selectedRecord.Due = dtpDue.Value.Date;

            selectedRecord.Returned = cboState.Text == "Đã trả"
                ? (DateTime?)dtpReturned.Value.Date
                : null;

            MessageBox.Show("Cập nhật phiếu thành công!");

            RefreshList();
            ClearForm();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedRecord == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu cần xóa!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa phiếu " +
                    selectedRecord.Id + "?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            data.Remove(selectedRecord);

            MessageBox.Show("Xóa phiếu thành công!");

            RefreshList();
            ClearForm();
        }

        private void btnMarkReturned_Click(
            object sender, EventArgs e)
        {
            if (selectedRecord == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn phiếu cần trả thiết bị!");
                return;
            }

            if (selectedRecord.Returned.HasValue)
            {
                MessageBox.Show("Phiếu này đã được trả!");
                return;
            }

            if (DateTime.Today < selectedRecord.Borrow.Date)
            {
                MessageBox.Show(
                    "Ngày trả không được trước ngày mượn!");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Xác nhận sinh viên đã trả thiết bị?",
                "Xác nhận trả thiết bị",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            selectedRecord.Returned = DateTime.Today;

            MessageBox.Show(
                "Đã xác nhận trả thiết bị thành công!");

            RefreshList();
            ClearForm();
        }

        private void dgvCheckout_SelectionChanged(
            object sender, EventArgs e)
        {
            if (loading)
                return;

            if (dgvCheckout.CurrentRow == null)
                return;

            Checkout checkout =
                dgvCheckout.CurrentRow.DataBoundItem as Checkout;

            if (checkout == null)
                return;

            selectedRecord = checkout;

            loading = true;

            try
            {
                txtId.Text = checkout.Id;
                txtStudentId.Text = checkout.StudentId;
                txtStudent.Text = checkout.Student;
                cboEquipment.SelectedItem = checkout.Equipment;
                txtQuantity.Text = checkout.Quantity.ToString();

                dtpBorrow.Value = checkout.Borrow;
                dtpDue.Value = checkout.Due;

                cboState.SelectedItem =
                    checkout.Returned.HasValue
                        ? "Đã trả"
                        : "Đang mượn";

                dtpReturned.Value =
                    checkout.Returned ?? DateTime.Today;

                dtpReturned.Checked =
                    checkout.Returned.HasValue;

                epCheck.Clear();
            }
            finally
            {
                loading = false;
            }
        }
        private void cboState_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (loading)
                return;

            if (cboState.Text == "Đã trả")
            {
                dtpReturned.Value = DateTime.Today;
                dtpReturned.Checked = true;
            }
            else
            {
                dtpReturned.Checked = false;
            }
        }
        private void RefreshList()
        {
            string keyword = txtSearch.Text.Trim();

            var result = data.Where(x =>
                (
                    keyword.Length == 0 ||
                    x.StudentId.IndexOf(keyword,
                        StringComparison.OrdinalIgnoreCase) >= 0 ||
                    x.Equipment.IndexOf(keyword,
                        StringComparison.OrdinalIgnoreCase) >= 0
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
                    new BindingList<Checkout>(result);

                dgvCheckout.ClearSelection();
                dgvCheckout.CurrentCell = null;
                selectedRecord = null;
            }
            finally
            {
                loading = false;
            }

            int total = data.Count;
            int visible = result.Count;
            int overdue = data.Count(x => x.State == "Quá hạn");

            lblStatus.Text =
                "Tổng phiếu: " + total +
                " | Đang hiển thị: " + visible +
                " | Phiếu quá hạn: " + overdue;
        }

        private void ClearForm()
        {
            loading = true;

            try
            {
                txtId.Clear();
                txtStudentId.Clear();
                txtStudent.Clear();
                txtQuantity.Text = "1";

                cboEquipment.SelectedIndex = 0;
                cboState.SelectedIndex = 0;

                dtpBorrow.Value = DateTime.Today;
                dtpDue.Value = DateTime.Today.AddDays(7);

                dtpReturned.Value = DateTime.Today;
                dtpReturned.Checked = false;

                selectedRecord = null;
                epCheck.Clear();

                dgvCheckout.ClearSelection();
                dgvCheckout.CurrentCell = null;
            }
            finally
            {
                loading = false;
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (source != null && cboFilter != null)
                RefreshList();
        }
    }
}
