using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ConvertExtension
    {
        #region 数据类型转换扩展方法
        /// <summary>
        /// object 转换成string 包括为空的情况
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>返回值不含空格</returns>
        public static string ToStringEx(this object obj)
        {
            return obj == null ? string.Empty : obj.ToString().Trim();
        }

        /// <summary>
        /// 时间object 转换成格式化的string 包括为空的情况
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="format"></param>
        /// <returns>返回值不含空格</returns>
        public static string TryToDateTimeToString(this object obj, string format)
        {
            if (obj == null)
                return string.Empty;
            DateTime dt;
            if (DateTime.TryParse(obj.ToString(), out dt))
                return dt.ToString(format);
            else
                return string.Empty;
        }

        /// <summary>
        /// 字符转Int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>成功:返回对应Int值;失败:返回0</returns>
        public static int TryToInt32(this object obj)
        {
            int rel = 0;

            if (!string.IsNullOrEmpty(obj.ToStringEx()))
            {
                int.TryParse(obj.ToStringEx(), out rel);
            }
            return rel;
        }

        /// <summary>
        /// 字符转Int64
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Int64 TryToInt64(this object obj)
        {
            Int64 rel = 0;
            if (!string.IsNullOrEmpty(obj.ToStringEx()))
            {
                Int64.TryParse(obj.ToStringEx(), out rel);
            }
            return rel;
        }

        /// <summary>
        /// 字符转DateTime
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>成功:返回对应Int值;失败:时间初始值</returns>
        public static DateTime TryToDateTime(this object obj)
        {
            DateTime rel = new DateTime();
            if (!string.IsNullOrEmpty(obj.ToStringEx()))
            {
                DateTime.TryParse(obj.ToStringEx(), out rel);
            }
            return rel;
        }

        /// <summary>
        /// 转换成Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string TryToPostJson(this object obj)
        {
            string rel = string.Empty;
            if (!string.IsNullOrEmpty(obj.ToStringEx()))
            {
                IsoDateTimeConverter iso = new IsoDateTimeConverter()
                {
                    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
                };
                rel = JsonConvert.SerializeObject(obj, iso);
            }
            return rel;
        }
        #endregion

        #region DataTable转换成List集合
        /// <summary>
        /// DataTable转换成List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> DataTableToList<T>(this DataTable dt) where T : new()
        {
            // 定义集合   
            IList<T> ts = new List<T>();

            // 获得此模型的类型  
            Type type = typeof(T);

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性     
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    string tempName = pi.Name;

                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter     
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                ts.Add(t);
            }
            return ts.CastTo<List<T>>();
        }
        #endregion

        #region List转换DataTable
        /// <summary>
        /// 将泛类型集合List类转换成DataTable
        /// </summary>
        /// <param name="entitys">泛类型集合</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(this List<T> entitys) where T : new()
        {
            //检查实体集合不能为空
            if (entitys == null || entitys.Count < 1)
            {
                throw new Exception("需转换的集合为空");
            }
            //取出第一个实体的所有Propertie
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            //生产代码中，应将生成的DataTable结构Cache起来，此处略
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                //dt.Columns.Add(entityProperties[i].Name, entityProperties[i].PropertyType);
                dt.Columns.Add(entityProperties[i].Name);
            }
            //将所有entity添加到DataTable中
            foreach (object entity in entitys)
            {
                //检查所有的的实体都为同一类型
                if (entity.GetType() != entityType)
                {
                    throw new Exception("要转换的集合元素类型不一致");
                }
                object[] entityValues = new object[entityProperties.Length];
                for (int i = 0; i < entityProperties.Length; i++)
                {
                    entityValues[i] = entityProperties[i].GetValue(entity, null);
                }
                dt.Rows.Add(entityValues);
            }
            return dt;
        }
        #endregion

        #region IList转成List<T>
        /// <summary>
        /// IList如何转成List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> IListToList<T>(this IList list) where T : new()
        {
            T[] array = new T[list.Count];
            list.CopyTo(array, 0);
            return new List<T>(array);
        }
        #endregion

        #region DataTable根据条件过滤表的内容
        /// <summary>
        /// 根据条件过滤表的内容
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static DataTable GetNewDataTable(this DataTable dt, string condition)
        {
            if (!IsExistRows(dt))
            {
                if (condition.Trim() == "")
                {
                    return dt;
                }
                else
                {
                    DataTable newdt = new DataTable();
                    newdt = dt.Clone();
                    DataRow[] dr = dt.Select(condition);
                    for (int i = 0; i < dr.Length; i++)
                    {
                        newdt.ImportRow((DataRow)dr[i]);
                    }
                    return newdt;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 检查DataTable 是否有数据行
        /// <summary>
        /// 检查DataTable 是否有数据行
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static bool IsExistRows(this DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
                return false;

            return true;
        }
        #endregion

        #region DataTable 转 DataTableToHashtable
        /// <summary>
        /// DataTable 转 DataTableToHashtable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Hashtable DataTableToHashtable(this DataTable dt)
        {
            Hashtable ht = new Hashtable();
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string key = dt.Columns[i].ColumnName;
                    ht[key] = dr[key];
                }
            }
            return ht;
        }
        #endregion

        #region DataTable/DataSet 转 XML
        /// <summary>
        /// DataTable 转 XML
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTableToXml(this DataTable dt)
        {
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    System.IO.StringWriter writer = new System.IO.StringWriter();
                    dt.WriteXml(writer);
                    return writer.ToString();
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// DataSet 转 XML
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DataSetToXml(this DataSet ds)
        {
            if (ds != null)
            {
                System.IO.StringWriter writer = new System.IO.StringWriter();
                ds.WriteXml(writer);
                return writer.ToString();
            }
            return String.Empty;
        }
        #endregion

        #region 将byte[]转换成int
        /// <summary>
        /// 将byte[]转换成int
        /// </summary>
        /// <param name="data">需要转换成整数的byte数组</param>
        public static int BytesToInt32(this byte[] data)
        {
            //如果传入的字节数组长度小于4,则返回0
            if (data.Length < 4)
            {
                return 0;
            }

            //定义要返回的整数
            int num = 0;

            //如果传入的字节数组长度大于4,需要进行处理
            if (data.Length >= 4)
            {
                //创建一个临时缓冲区
                byte[] tempBuffer = new byte[4];

                //将传入的字节数组的前4个字节复制到临时缓冲区
                Buffer.BlockCopy(data, 0, tempBuffer, 0, 4);

                //将临时缓冲区的值转换成整数，并赋给num
                num = BitConverter.ToInt32(tempBuffer, 0);
            }

            //返回整数
            return num;
        }
        #endregion

        #region 补足位数
        /// <summary>
        /// 指定字符串的固定长度，如果字符串小于固定长度，
        /// 则在字符串的前面补足零，可设置的固定长度最大为9位
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <param name="limitedLength">字符串的固定长度</param>
        public static string RepairZero(this string text, int limitedLength)
        {
            //补足0的字符串
            string temp = "";

            //补足0
            for (int i = 0; i < limitedLength - text.Length; i++)
            {
                temp += "0";
            }

            //连接text
            temp += text;

            //返回补足0的字符串
            return temp;
        }

        /// <summary>
        /// 小时、分钟、秒小于10补足0
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <returns></returns>
        public static string RepairZero(this int text)
        {
            string res = string.Empty;
            if (text >= 0 && text < 10)
            {
                res += "0" + text;
            }
            else
            {
                res = text.ToString();
            }
            return res;
        }

        #endregion

        #region 各进制数间转换
        /// <summary>
        /// 实现各进制数间的转换。ConvertBase("15",10,16)表示将十进制数15转换为16进制的数。
        /// </summary>
        /// <param name="value">要转换的值,即原值</param>
        /// <param name="from">原值的进制,只能是2,8,10,16四个值。</param>
        /// <param name="to">要转换到的目标进制，只能是2,8,10,16四个值。</param>
        public static string ConvertBase(this string value, int from, int to)
        {
            try
            {
                int intValue = Convert.ToInt32(value, from);  //先转成10进制
                string result = Convert.ToString(intValue, to);  //再转成目标进制
                if (to == 2)
                {
                    int resultLength = result.Length;  //获取二进制的长度
                    switch (resultLength)
                    {
                        case 7:
                            result = "0" + result;
                            break;
                        case 6:
                            result = "00" + result;
                            break;
                        case 5:
                            result = "000" + result;
                            break;
                        case 4:
                            result = "0000" + result;
                            break;
                        case 3:
                            result = "00000" + result;
                            break;
                    }
                }
                return result;
            }
            catch
            {

                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
                return "0";
            }
        }
        #endregion

        #region 使用指定字符集将string转换成byte[]
        /// <summary>
        /// 使用指定字符集将string转换成byte[]
        /// </summary>
        /// <param name="text">要转换的字符串</param>
        /// <param name="encoding">字符编码</param>
        public static byte[] StringToBytes(this string text, Encoding encoding)
        {
            return encoding.GetBytes(text);
        }
        #endregion

        #region 使用指定字符集将byte[]转换成string
        /// <summary>
        /// 使用指定字符集将byte[]转换成string
        /// </summary>
        /// <param name="bytes">要转换的字节数组</param>
        /// <param name="encoding">字符编码</param>
        public static string BytesToString(this byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }
        #endregion

        #region IQueryable<T>的扩展方法

        //#region 根据第三方条件是否为真是否执行指定条件的查询
        ///// <summary>
        ///// 根据第三方条件是否为真是否执行指定条件的查询
        ///// </summary>
        ///// <typeparam name="T">动态类型</typeparam>
        ///// <param name="source">要查询的数据源</param>
        ///// <param name="where">条件</param>
        ///// <param name="condition">第三方条件</param>
        ///// <returns>查询的结果</returns>
        //public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> where,
        //    bool condition)
        //{
        //    if (PublicHelper.CheckArgument(where, "where"))
        //    {
        //        return condition ? source.Where(where) : source;
        //    }
        //    return null;
        //}
        //#endregion

        //#region 把IQueryable<T>集合按照指定的属性与排序方式进行排序
        ///// <summary>
        ///// 把IQueryable集合按照指定的属性与排序方式进行排序
        ///// </summary>
        ///// <typeparam name="T">数据集类型</typeparam>
        ///// <param name="source">要排序的数据集</param>
        ///// <param name="propName">要排序的属性名称</param>
        ///// <param name="listSort">排序方式</param>
        ///// <returns>返回排序后的结果集</returns>
        //public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propName,
        //    ListSortDirection listSort = ListSortDirection.Ascending)
        //{
        //    if (PublicHelper.CheckArgument(propName, "propName"))
        //    {
        //        return QueryableHelper<T>.OrderBy(source, propName, listSort);
        //    }
        //    return null;
        //}
        //#endregion

        //#region 把IQueryable<T>集合按照指定的属性与排序方式进行排序
        ///// <summary>
        ///// 把IQueryable集合按照指定的属性与排序方式进行排序
        ///// </summary>
        ///// <typeparam name="T">数据集类型</typeparam>
        ///// <param name="source">要排序的数据集</param>
        ///// <param name="sortCondition">排序条件</param>
        ///// <returns>返回排序后的结果集</returns>
        //public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, PropertySortCondition sortCondition)
        //{
        //    if (PublicHelper.CheckArgument(sortCondition, "sortCondition"))
        //    {
        //        return source.OrderBy(sortCondition.PropertyName, sortCondition.ListSortDirection);
        //    }
        //    return null;
        //}
        //#endregion

        //#region 把IOrderedQueryable<T>集合按照指定的属性与排序方式进行再排序
        ///// <summary>
        ///// 把IOrderedQueryable集合按照指定的属性与排序方式进行排序
        ///// </summary>
        ///// <typeparam name="T">数据集类型</typeparam>
        ///// <param name="source">要排序的数据集</param>
        ///// <param name="propName">要排序的属性名称</param>
        ///// <param name="listSort">排序方式</param>
        ///// <returns>返回排序后的结果集</returns>
        //public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propName,
        //    ListSortDirection listSort = ListSortDirection.Ascending)
        //{
        //    if (PublicHelper.CheckArgument(propName, "propName"))
        //    {
        //        return QueryableHelper<T>.ThenBy(source, propName, listSort);
        //    }
        //    return null;
        //}
        //#endregion

        //#region 把IOrderedQueryable<T>集合按照指定的属性与排序方式进行再排序
        ///// <summary>
        ///// 把IOrderedQueryable集合按照指定的属性与排序方式进行排序
        ///// </summary>
        ///// <typeparam name="T">数据集类型</typeparam>
        ///// <param name="source">要排序的数据集</param>
        ///// <param name="sortCondition">排序条件</param>
        ///// <returns>返回排序后的结果集</returns>
        //public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, PropertySortCondition sortCondition)
        //{
        //    if (PublicHelper.CheckArgument(sortCondition, "sortCondition"))
        //    {
        //        return QueryableHelper<T>.ThenBy(source, sortCondition.PropertyName, sortCondition.ListSortDirection);
        //    }
        //    return null;
        //}
        //#endregion

        //#region 多条件排序分页查询
        ///// <summary>
        ///// 把IQueryable集合按照指定的属性与排序方式进行排序后，再按照指定的条件提取指定页码指定条目数据
        ///// </summary>
        ///// <typeparam name="T">动态类型</typeparam>
        ///// <param name="source">要排序的数据集</param>
        ///// <param name="where">检索条件</param>
        ///// <param name="pageIndex">索引</param>
        ///// <param name="pageSize">页面大小</param>
        ///// <param name="total">总页数</param>
        ///// <param name="sortConditions">排序条件</param>
        ///// <returns>子集</returns>
        //public static IQueryable<T> Where<T>(this IQueryable<T> source, Expression<Func<T, bool>> where, int pageIndex,
        //    int pageSize, out int total, params PropertySortCondition[] sortConditions) where T : class, new()
        //{
        //    IQueryable<T> temp = null;

        //    int i = 0;
        //    if (PublicHelper.CheckArgument(source, "source")
        //            && PublicHelper.CheckArgument(where, "where")
        //            && PublicHelper.CheckArgument(pageIndex, "pageIndex")
        //            && PublicHelper.CheckArgument(pageSize, "pageSize")
        //            && PublicHelper.CheckArgument(sortConditions, "sortConditions"))
        //    {
        //        //判断是不是首个排序条件
        //        int count = 0;
        //        //得到满足条件的总记录数
        //        i = source.Count(where);
        //        //对数据源进行排序
        //        IOrderedQueryable<T> orderSource = null;
        //        foreach (PropertySortCondition sortCondition in sortConditions)
        //        {
        //            orderSource = count == 0
        //                ? source.OrderBy(sortCondition.PropertyName, sortCondition.ListSortDirection)
        //                : orderSource.ThenBy(sortCondition.PropertyName, sortCondition.ListSortDirection);
        //            count++;
        //        }
        //        source = orderSource;

        //        temp = source != null
        //            ? source.Where(where).Skip((pageIndex - 1) * pageSize).Skip(pageSize)
        //            : Enumerable.Empty<T>().AsQueryable();
        //    }

        //    total = i;
        //    return temp;

        //    //PublicHelper.CheckArgument(source, "source");
        //    //PublicHelper.CheckArgument(where, "where");
        //    //PublicHelper.CheckArgument(pageIndex, "pageIndex");
        //    //PublicHelper.CheckArgument(pageSize, "pageSize");
        //    //PublicHelper.CheckArgument(sortConditions, "sortConditions");

        //    ////判断是不是首个排序条件
        //    //int count = 0;
        //    ////得到满足条件的总记录数
        //    //total = source.Count(where);
        //    ////对数据源进行排序
        //    //IOrderedQueryable<T> orderSource = null;
        //    //foreach (PropertySortCondition sortCondition in sortConditions)
        //    //{
        //    //    orderSource = count == 0
        //    //        ? source.OrderBy(sortCondition.PropertyName, sortCondition.ListSortDirection)
        //    //        : orderSource.ThenBy(sortCondition.PropertyName, sortCondition.ListSortDirection);
        //    //    count++;
        //    //}
        //    //source = orderSource;

        //    //return source != null
        //    //    ? source.Where(where).Skip((pageIndex - 1) * pageSize).Skip(pageSize)
        //    //    : Enumerable.Empty<T>().AsQueryable();
        //}
        //#endregion

        //#region 多条件排序查询
        /////// <summary>
        /////// 多条件排序查询
        /////// </summary>
        /////// <typeparam name="T">动态类型</typeparam>
        /////// <param name="source">要排序的数据集</param>
        /////// <param name="where">检索条件</param>
        /////// <param name="sortConditions">排序条件</param>
        /////// <returns>子集</returns>
        ////public static IQueryable<T> Where<T>(this IQueryable<T> source, Expression<Func<T, bool>> where, params PropertySortCondition[] sortConditions) where T : class, new()
        ////{
        ////    log = LogManager.GetLogger(string.Format("查询表_{0}", typeof(T)));
        ////    IQueryable<T> temp = null;
        ////    Logger("", () =>
        ////    {
        ////        if (PublicHelper.CheckArgument(source, "source")
        ////            && PublicHelper.CheckArgument(where, "where")
        ////            && PublicHelper.CheckArgument(sortConditions, "sortConditions"))
        ////        {
        ////            //判断是不是首个排序条件
        ////            int count = 0;
        ////            //对数据源进行排序
        ////            IOrderedQueryable<T> orderSource = null;
        ////            foreach (PropertySortCondition sortCondition in sortConditions)
        ////            {
        ////                orderSource = count == 0
        ////                    ? source.OrderBy(sortCondition.PropertyName, sortCondition.ListSortDirection)
        ////                    : orderSource.ThenBy(sortCondition.PropertyName, sortCondition.ListSortDirection);
        ////                count++;
        ////            }
        ////            source = orderSource;

        ////            temp = source != null
        ////                ? source.Where(where)
        ////                : Enumerable.Empty<T>().AsQueryable();
        ////        }
        ////    });
        ////    return temp;
        ////}
        //#endregion

        #endregion

        #region IEnumerable<T>的扩展方法

        #region 将集合展开分别转换成字符串，再以指定分隔字符串链接，拼成一个字符串返回
        /// <summary>
        /// 将集合展开分别转换成字符串，再以指定分隔字符串链接，拼成一个字符串返回
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="collection">要处理的集合</param>
        /// <param name="separator">分隔符</param>
        /// <returns>拼接后的字符串</returns>
        public static string ExpandAndToString<T>(this IEnumerable<T> collection, string separator)
        {
            List<T> source = collection as List<T> ?? collection.ToList();
            if (source.IsEmpty())
            {
                return "";
            }

            string result = source.Cast<object>()
                 .Aggregate<object, string>(null, (current, o) => current + String.Format("{0}{1}", o, separator));

            return result.Substring(0, result.LastIndexOf(separator, StringComparison.Ordinal));
        }
        #endregion

        #region 根据指定条件返回集合中不重复的元素
        /// <summary>
        /// 根据指定条件返回集合中不重复的元素
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <typeparam name="TKey">动态筛选条件类型</typeparam>
        /// <param name="source">要操作的数据源</param>
        /// <param name="keySelector">重复数据的筛选条件</param>
        /// <returns>不重复元素的集合</returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            //取分组集合中每组的第一条数据
            return source.GroupBy(keySelector).Select(group => group.First());
        }
        #endregion

        #region 根据第三方条件是否为真是否执行指定条件的查询
        /// <summary>
        /// 根据第三方条件是否为真是否执行指定条件的查询
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="source">要查询的数据源</param>
        /// <param name="where">条件</param>
        /// <param name="condition">第三方条件</param>
        /// <returns>查询的结果</returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> where,
            bool condition)
        {
            return condition ? source.Where(where) : source;
        }
        #endregion

        #region 判断集合是否为空
        /// <summary>
        /// 判断集合是否为空
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="collection">要处理的集合</param>
        /// <returns>集合为空返回true 不为空返回false</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return !collection.Any();
        }
        #endregion

        #endregion

        #region 其他
        /// <summary>
        /// 扩展Between 操作符
        /// 使用 var query = db.People.Between(person => person.Age, 18, 21);
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source">当前值</param>
        /// <param name="keySelector">拉姆达表达式</param>
        /// <param name="low">低</param>
        /// <param name="high">高</param>
        /// <returns></returns>
        public static IQueryable<TSource> Between<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, TKey low, TKey high) where TKey : IComparable<TKey>
        {
            Expression key = Expression.Invoke(keySelector, keySelector.Parameters.ToArray());

            Expression lowerBound = Expression.GreaterThanOrEqual(key, Expression.Constant(low));

            Expression upperBound = Expression.LessThanOrEqual(key, Expression.Constant(high));

            Expression and = Expression.AndAlso(lowerBound, upperBound);

            Expression<Func<TSource, bool>> lambda = Expression.Lambda<Func<TSource, bool>>(and, keySelector.Parameters);

            return source.Where(lambda);
        }

        /// <summary>
        /// In
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool In<T>(this T value, params T[] values)
        {
            return values.Contains(value);
        }

        /// <summary>
        /// Between
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool Between<T>(this T i, T start, T end) where T : IComparable<T>
        {
            return i.CompareTo(start) >= 0 && i.CompareTo(end) <= 0;
        }

        /// <summary>
        /// And
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool And(this bool left, bool right)
        {
            return left && right;
        }

        /// <summary>
        /// Or
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Or(this bool left, bool right)
        {
            return left || right;
        }

        #endregion
    }
}
