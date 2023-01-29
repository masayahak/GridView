using GridPager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewModel;

namespace GridView
{

    public partial class FormGridView : Form
    {
        public readonly bool isデバッグ = true;

        private VM売上 vm売上;

        public FormGridView()
        {
            InitializeComponent();

            dtp開始.Value = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Local);
            dtp終了.Value = new DateTime(2022, 12, 31, 0, 0, 0, DateTimeKind.Local);
        }

        private void Form高速GridView_Load(object sender, EventArgs e)
        {
            // DataGirdViewのパフォーマンス・チューニング
            InitDataGridView();

            pictureBoxロード中.Visible = false;
        }

        // ----------------------------------------------------------------
        // GridView 高速化用の設定
        // ----------------------------------------------------------------
        private void InitDataGridView()
        {
            Type dgvtype = typeof(DataGridView);

            // プロパティ設定の取得
            System.Reflection.PropertyInfo dgvPropertyInfo =
                dgvtype.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            dgvPropertyInfo.SetValue(dataGridView, true, null);

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView.RowHeadersVisible = false;
        }


        private void btn検索_Click(object sender, EventArgs e)
        {
            DataLoad();
        }

        // ----------------------------------------------------------------
        // データ読み込み
        // ----------------------------------------------------------------

        private async void DataLoad()
        {

            pictureBoxロード中.Visible = true;

            vm売上 = new VM売上();

            DateTime 期間開始 = this.dtp開始.Value.Date;
            DateTime 期間終了 = this.dtp終了.Value.Date;

            // =======================================================
            // 非同期でデータ取得
            // =======================================================

            // Stopwatchクラス生成
            var sw = new System.Diagnostics.Stopwatch();
            // 計測開始
            sw.Start();

            // ------------ 非同期処理 ---------------
            var 売上s = new List<T売上>();
            await Task.Run(() =>
            {
                売上s = vm売上.Get売上s(期間開始, 期間終了);
            });

            // 計測停止
            sw.Stop();

            if (isデバッグ)
            {
                var DBの時間 = Math.Round(sw.Elapsed.TotalMilliseconds/1000, 3).ToString();
                Console.WriteLine("DB処理時間 {0}秒", DBの時間);
            }

            // 件数の表示
            var 件数表示 = 売上s.Count().ToString("#,##0 件");
            this.lbl件数.Text = 件数表示;

            // 計測開始
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            // 取得したデータをGridへ表示
            BindingSource bs = new BindingSource();
            bs.DataSource = 売上s;
            dataGridView.DataSource = bs;

            // 見栄えの調整
            Grid列幅調整();

            // 計測停止
            sw.Stop();

            if (isデバッグ)
            {
                var Grid表示時間 = Math.Round(sw.Elapsed.TotalMilliseconds / 1000, 3).ToString();
                Console.WriteLine("Grid表示時間 {0}秒", Grid表示時間);
            }

            pictureBoxロード中.Visible = false;
        }

        private void Grid列幅調整()
        {
            // 列幅の設定:
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                if (column.ValueType == typeof(int))
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (column.ValueType == typeof(DateTime))
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

    }
}
