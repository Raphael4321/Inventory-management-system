﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gerenciador_de_estoque.src.Controllers;
using Gerenciador_de_estoque.src.Models;
using Gerenciador_de_estoque.src.Utilities;

namespace Gerenciador_de_estoque.src.UI
{
    public partial class SupplierMenu : Form
    {
        private Supplier supplier;
        private List<Supplier> suppliers;
        private readonly SupplierController _controller;
        private readonly bool _isSelecting;
        private readonly Utils _utils;

        public event Action<Supplier> SupplierSelected;

        public SupplierMenu(bool isSelecting)
        {
            try
            {
                suppliers = new List<Supplier>();
                supplier = new Supplier();

                _controller = new SupplierController();
                _utils = new Utils();
                _isSelecting = isSelecting;

                InitializeComponent();

                InitializeForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar o menu do fornecedor: {ex.Message}");
            }
        }

        private void InitializeForm()
        {
            try
            {
                HandleFields(_isSelecting, true);
                AddColumns();
                FillDataGridView(TxtSearch.Text, true);
                FillCmbStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar o formulário: {ex.Message}");
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FillDataGridView(TxtSearch.Text, false);
        }

        private void TxtName_TextChanged(object sender, EventArgs e) { }

        private void TxtPhone_TextChanged(object sender, EventArgs e)
        {
            TxtPhone.Text = _utils.FormatPhone(TxtPhone.Text);
        }

        private void TxtCEP_TextChanged(object sender, EventArgs e)
        {
            TxtCEP.Text = _utils.FormatCEP(TxtCEP.Text);
        }

        private void TxtNumber_TextChanged(object sender, EventArgs e)
        {
            TxtNumber.Text = _utils.ValidateNumber(TxtNumber.Text);
        }

        private void DtSupplier_SelectionChanged(object sender, EventArgs e)
        {
            SelectRow();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            CleanSupplier();
            HandleFields(_isSelecting, false);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSupplierObj();
                SaveSupplier();
                FillDataGridView(TxtSearch.Text, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar o fornecedor: {ex.Message}");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            HandleFields(_isSelecting, false);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            HandleFields(_isSelecting, true);
        }

        private void BtnGoBack_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao voltar: {ex.Message}");
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (supplier != null)
            {
                SupplierSelected?.Invoke(supplier);
            }
            Close();
        }

        private void HandleFields(bool isSelecting, bool isReadOnly)
        {
            TxtName.Text = supplier.Name ?? "";
            TxtCity.Text = supplier.City ?? "";
            TxtCEP.Text = supplier.CEP ?? "";
            TxtNeigh.Text = supplier.Neighborhood ?? "";
            TxtPhone.Text = supplier.Phone ?? "";
            TxtStreet.Text = supplier.Street ?? "";
            TxtEmail.Text = supplier.Email ?? "";
            TxtNumber.Text = supplier.Number ?? "";
            TxtComplement.Text = supplier.Complement ?? "";
            CmbStates.SelectedItem = supplier.State ?? null;

            UpdateButtons(isSelecting, isReadOnly);

            SetFieldReadOnlyStatus(isReadOnly);
        }

        private void SelectRow()
        {
            supplier = _utils.SelectRowSupplier(DtSupplier);
            HandleFields(_isSelecting, true);
        }

        private void SaveSupplier()
        {
            if (_controller.ValidateSupplier(supplier).Count <= 0)
            {
                if (_controller.AddSupplier(supplier))
                {
                    MessageBox.Show("Fornecedor salvo com sucesso!");
                }
            }
            else
            {
                throw new ArgumentException(
                    "Preencha os campos a seguir antes de continuar: "
                        + string.Join(", ", _controller.ValidateSupplier(supplier))
                );
            }
        }

        private void AddColumns()
        {
            _utils.AddSupplierColumns(DtSupplier);
        }

        private void FillDataGridView(string name, bool dbchange)
        {
            GatherSuppliers(dbchange);
            List<Supplier> filtered = FilterSuppliers(name);
            FillSuppierTable(filtered);
        }

        private void GatherSuppliers(bool dbchange)
        {
            if (suppliers.Count <= 0 || dbchange)
                suppliers = _controller.GatherSuppliers();
        }

        private List<Supplier> FilterSuppliers(string name)
        {
            return _utils.FilterSupplierList(suppliers, name);
        }

        private void FillSuppierTable(List<Supplier> list)
        {
            DtSupplier.Rows.Clear();

            foreach (var fornecedor in list)
            {
                DtSupplier.Rows.Add(
                    fornecedor.Id,
                    fornecedor.Name,
                    fornecedor.Phone,
                    fornecedor.CEP,
                    fornecedor.Neighborhood,
                    fornecedor.Street,
                    fornecedor.Email,
                    fornecedor.Number,
                    fornecedor.Complement,
                    fornecedor.City,
                    fornecedor.State
                );
            }
        }

        private void FillCmbStates()
        {
            CmbStates.Items.AddRange(new Utils().ListStates().ToArray());
        }

        private void SetFieldReadOnlyStatus(bool isReadOnly)
        {
            TxtName.ReadOnly = isReadOnly;
            TxtCity.ReadOnly = isReadOnly;
            TxtCEP.ReadOnly = isReadOnly;
            TxtNeigh.ReadOnly = isReadOnly;
            TxtPhone.ReadOnly = isReadOnly;
            TxtStreet.ReadOnly = isReadOnly;
            TxtEmail.ReadOnly = isReadOnly;
            TxtNumber.ReadOnly = isReadOnly;
            TxtComplement.ReadOnly = isReadOnly;
            CmbStates.Enabled = !isReadOnly;
        }

        private void UpdateButtons(bool isSelecting, bool isEnabled)
        {
            if (isSelecting == true)
            {
                BtnSelect.Enabled = isSelecting;
                BtnSelect.Visible = isSelecting;
                BtnNew.Enabled = !isSelecting;
                BtnNew.Visible = !isSelecting;
                BtnEdit.Enabled = !isSelecting;
                BtnEdit.Visible = !isSelecting;
                BtnSave.Enabled = !isSelecting;
                BtnCancel.Enabled = !isSelecting;
                BtnCancel.Visible = !isSelecting;
                BtnSave.Visible = !isSelecting;
                BtnCancel.Visible = !isSelecting;
                BtnCancel.Enabled = !isSelecting;
                BtnGoBack.Enabled = !isSelecting;
                BtnGoBack.Visible = !isSelecting;
            }
            else
            {
                BtnSelect.Enabled = false;
                BtnSelect.Visible = false;
                BtnNew.Enabled = isEnabled;
                BtnNew.Visible = isEnabled;
                BtnEdit.Enabled = isEnabled;
                BtnEdit.Visible = isEnabled;
                BtnSave.Enabled = !isEnabled;
                BtnCancel.Enabled = !isEnabled;
                BtnCancel.Visible = !isEnabled;
                BtnSave.Visible = !isEnabled;
            }
        }

        private void UpdateSupplierObj()
        {
            supplier.Name = TxtName.Text;
            supplier.City = TxtCity.Text;
            supplier.CEP = TxtCEP.Text;
            supplier.Neighborhood = TxtNeigh.Text;
            supplier.Phone = TxtPhone.Text;
            supplier.Street = TxtStreet.Text;
            supplier.Email = TxtEmail.Text;
            supplier.Number = TxtNumber.Text;
            supplier.Complement = TxtComplement.Text;
            supplier.State = CmbStates.SelectedItem?.ToString();
        }

        private void CleanSupplier()
        {
            supplier = new Supplier();
        }
    }
}
