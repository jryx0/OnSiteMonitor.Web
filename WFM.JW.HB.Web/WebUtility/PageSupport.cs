using System;

namespace WFM.JW.HB.Web
{
   
    [Serializable]
    public class PageSupport 
    {
        private int _eachPageTotal;
        private bool _needTotalCount;
        private int _startIndex;
        private int _totalCount;

        public PageSupport()
        {
            this._totalCount = 0;
            this._startIndex = 0;
            this._eachPageTotal = 20;
            this._needTotalCount = false;
            this._eachPageTotal = 20;
        }

        public PageSupport(int eachPageTotal)
        {
            this._totalCount = 0;
            this._startIndex = 0;
            this._eachPageTotal = 20;
            this._needTotalCount = false;
            if (eachPageTotal < 1)
            {
                this._eachPageTotal = 20;
            }
            else
            {
                this._eachPageTotal = eachPageTotal;
            }
        }

        public PageSupport(int eachPageTotal, int startIndex, int totalCount)
        {
            this._totalCount = 0;
            this._startIndex = 0;
            this._eachPageTotal = 20;
            this._needTotalCount = false;
            this._startIndex = startIndex;
            if (this._startIndex < 0)
            {
                this._startIndex = 0;
            }
            this.TotalCount = totalCount;
            this.EachPageTotal = eachPageTotal;
        }

        public int GetPageStartIndex(int pageNo)
        {
            if (pageNo > this.TotalPagesCount)
            {
                pageNo = this.TotalPagesCount;
            }
            return (this.EachPageTotal * ((pageNo > 0) ? (pageNo - 1) : 0));
        }

        public void SetStartIndex(int startIndex)
        {
            this._startIndex = startIndex;
        }

        public int CurrentPageEndIndex
        {
            get
            {
                int endIndex = this.CurrentPageStartIndex + this.EachPageTotal;
                return ((endIndex > this.TotalCount) ? this.TotalCount : endIndex);
            }
        }

        public int CurrentPageNo
        {
            get
            {
                int i = (int) Math.Ceiling((double) (((double) (this.CurrentPageStartIndex + 1)) / ((double) this.EachPageTotal)));
                if (i > this.TotalPagesCount)
                {
                    i = this.TotalPagesCount;
                    this._startIndex = this.LastPageStartIndex;
                }
                return i;
            }
        }

        public int CurrentPageStartIndex
        {
            get
            {
                return this._startIndex;
            }
            set
            {
                this._startIndex = value;
                if (this._startIndex < 0)
                {
                    this._startIndex = 0;
                }
            }
        }

        public int EachPageTotal
        {
            get
            {
                return this._eachPageTotal;
            }
            set
            {
                this._eachPageTotal = (value < 1) ? 20 : value;
            }
        }

        public int LastPageStartIndex
        {
            get
            {
                int i = this.TotalCount - (this.TotalCount % this.EachPageTotal);
                if (i == this.TotalCount)
                {
                    i = this.TotalCount - this.EachPageTotal;
                }
                return ((i < 0) ? 0 : i);
            }
        }

        public bool NeedTotalCount
        {
            get
            {
                return this._needTotalCount;
            }
            set
            {
                this._needTotalCount = value;
            }
        }

        public int NextPageIndex
        {
            get
            {
                int[] nidxes = this.NextStartIndexes;
                return ((nidxes.Length == 0) ? -1 : nidxes[0]);
            }
        }

        public int[] NextStartIndexes
        {
            get
            {
                int endIndex = this.CurrentPageEndIndex;
                if (endIndex == this.TotalCount)
                {
                    return new int[0];
                }
                int pageCounts = (int) Math.Ceiling((double) ((this.TotalCount - endIndex) / this.EachPageTotal));
                int[] result = new int[pageCounts];
                for (int i = 0; i < pageCounts; i++)
                {
                    if (i > 0)
                    {
                        endIndex += this.EachPageTotal;
                    }
                    result[i] = endIndex + 1;
                }
                return result;
            }
        }

        public int PreviousPageIndex
        {
            get
            {
                int[] pidxes = this.PreviousStartIndexes;
                return ((pidxes.Length == 0) ? -1 : pidxes[pidxes.Length - 1]);
            }
        }

        public int[] PreviousStartIndexes
        {
            get
            {
                int startIndex = this.CurrentPageStartIndex;
                if (startIndex == 0)
                {
                    return new int[0];
                }
                int count = (int) Math.Ceiling((double) (((double) startIndex) / ((double) this.EachPageTotal)));
                int[] result = new int[count];
                for (int i = count - 1; i >= 0; i--)
                {
                    if (i > 0)
                    {
                        startIndex -= this.EachPageTotal;
                    }
                    result[i] = startIndex - 1;
                }
                return result;
            }
        }

        public int TotalCount
        {
            get
            {
                return this._totalCount;
            }
            set
            {
                this._totalCount = (value < 0) ? 0 : value;
            }
        }

        public int TotalPagesCount
        {
            get
            {
                if (this.TotalCount <= 0)
                {
                    return 0;
                }
                return (int) Math.Ceiling((double) (((double) this.TotalCount) / ((double) this.EachPageTotal)));
            }
        }
    }
}

