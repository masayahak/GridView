using GridPager.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModel
{
    public class VM売上
    {

        // -------------------------------------------------------------------------
        // T売上の読み込み
        // -------------------------------------------------------------------------
        public List<T売上> Get売上s(DateTime 期間開始, DateTime 期間終了)
        {
            List<T売上> list売上 = new List<T売上>();

            using (var db = new MyDbContext())
            {
                list売上 = db.T売上s
                            .Where(it => it.売上日 >= 期間開始 && it.売上日 <= 期間終了)
                            .OrderBy(it => it.売上日)
                            .ThenBy(it => it.得意先コード)
                            .ToList();
            }
            return list売上;
        }
    }
}
