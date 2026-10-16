using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClinicAppointmentManager
{
    public partial class Form1 : Form
    {
        private BindingList<Appointment> data = new BindingList<Appointment>();

        private BindingSource source = new BindingSource();
        private ErrorProvider error = new ErrorProvider();

        public Form1()
        {
            InitializeComponent();

            error.ContainerControl = this;
            source.DataSource = data;
            grid.DataSource = source;

            add.Click += (s, e) => Save(false);
            edit.Click += (s, e) => Save(true);
            del.Click += (s, e) => Delete();
            clear.Click += (s, e) => Clear();

            grid.SelectionChanged += (s, e) => LoadSelected();
            search.TextChanged += (s, e) => RefreshList();
            filter.SelectedIndexChanged += (s, e) => RefreshList();
            day.ValueChanged += (s, e) => RefreshList();
            useDay.CheckedChanged += (s, e) => RefreshList();

            exportMenuItem.Click += (s, e) => Ui.Export(grid);

            exitMenuItem.Click += (s, e) => Close();

            when.Value = DateTime.Now.AddHours(1);
            RefreshList();
        }

        private Appointment Selected()
        {
            return grid.CurrentRow == null
                ? null
                : grid.CurrentRow.DataBoundItem as Appointment;
        }

        private void LoadSelected()
        {
            var a = Selected();
            if (a == null) return;

            id.Text = a.Id;
            patient.Text = a.Patient;
            phone.Text = a.Phone;
            when.Value = a.When;
            doctor.SelectedItem = a.Doctor;
            reason.Text = a.Reason;
            status.SelectedItem = a.Status;

            error.Clear();
        }

        private void Clear()
        {
            id.Clear();
            patient.Clear();
            phone.Clear();
            reason.Clear();

            doctor.SelectedIndex = 0;
            status.SelectedIndex = 0;
            when.Value = DateTime.Now.AddHours(1);

            error.Clear();
            Ui.ResetSelection(grid);
        }

        private bool ValidateInput(bool updating)
        {
            error.Clear();
            bool ok = true;

            Action<Control, string> bad = (c, message) =>
            {
                error.SetError(c, message);
                ok = false;
            };

            string code = id.Text.Trim();

            if (code.Length == 0)
                bad(id, "Nhập mã lịch hẹn");
            else if (data.Any(a =>
                a.Id.Equals(code,
                    StringComparison.OrdinalIgnoreCase)
                && (!updating || a != Selected())))
            {
                bad(id, "Mã lịch hẹn đã tồn tại");
            }

            if (patient.Text.Trim().Length == 0)
                bad(patient, "Nhập tên bệnh nhân");

            if (!Ui.Phone(phone.Text.Trim()))
                bad(phone, "SĐT phải có 10 chữ số, bắt đầu bằng 0");

            if (reason.Text.Trim().Length == 0)
                bad(reason, "Nhập lý do khám");

            if (when.Value < DateTime.Now)
                bad(when, "Ngày giờ hẹn không được trong quá khứ");

            return ok;
        }

        private void Save(bool updating)
        {
            var existing = Selected();

            if (updating && existing == null)
            {
                MessageBox.Show("Chọn lịch hẹn cần sửa");
                return;
            }

            if (!ValidateInput(updating)) return;

            var a = updating ? existing : new Appointment();

            a.Id = id.Text.Trim();
            a.Patient = patient.Text.Trim();
            a.Phone = phone.Text.Trim();
            a.When = when.Value;
            a.Doctor = doctor.Text;
            a.Reason = reason.Text.Trim();
            a.Status = status.Text;

            if (!updating)
                data.Add(a);

            RefreshList();
            Clear();

            MessageBox.Show(
                updating ? "Cập nhật thành công!" :
                           "Thêm lịch hẹn thành công!");
        }

        private void Delete()
        {
            var a = Selected();

            if (a == null)
            {
                MessageBox.Show("Chọn lịch hẹn cần xóa");
                return;
            }

            if (Ui.Confirm("lịch hẹn " + a.Id))
            {
                data.Remove(a);
                RefreshList();
                Clear();
                MessageBox.Show("Xóa thành công!");
            }
        }

        private void RefreshList()
        {
            string q = search.Text.Trim();

            var result = data.Where(a =>
                (
                    q.Length == 0 ||
                    a.Patient.IndexOf(
                        q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    a.Phone.Contains(q)
                )
                &&
                (
                    !useDay.Checked ||
                    a.When.Date == day.Value.Date
                )
                &&
                (
                    filter.SelectedIndex <= 0 ||
                    a.Status == filter.Text
                )
            ).ToList();

            source.DataSource  =new BindingList<Appointment>(result);
             count.Text ="Số lịch hẹn: " + result.Count + " / " + data.Count;
            Ui.ResetSelection(grid);
        }
    }
}
