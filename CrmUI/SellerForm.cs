using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrmBl.Model;

namespace CrmUI
{
    public partial class SellerForm : Form
    {
        public Seller Seller { get; set; }
        public SellerForm()
        {
            InitializeComponent();
        }
        public SellerForm(Seller seller):this()
        {
            Seller = seller;
            textBox1.Text = Seller.Name;
        }
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var s = Seller ?? new Seller();
                s.Name = textBox1.Text;
            Close();
        }
    }
}
