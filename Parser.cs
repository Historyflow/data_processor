using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBproc
{
    public enum ParserStatusType
    {
        TEMPLATE = 0x0,    // {{
        LINK = 0x1,      // [[
        TEXT = 0x2,
        IMAGE = 0x4
    };

    public class ParsedElement
    {
        public string Address = "";
        public int id;
        public string Label = ""; // null не нужен
        public ParserStatusType status;
        public List<ParsedElement> sub;

        public ParsedElement(string s)
        { // плоский текст
            Label = s;
            status = ParserStatusType.TEXT;
        }

        public ParsedElement(ParserStatusType pst)
        { // плоский текст
            status = pst;
        }

        public bool ParseTemplate()
        { // плоский текст
            if (status != ParserStatusType.TEMPLATE)
                return false; // пока - не положено
            if (sub == null || sub.Count == 0)
                return false;
            /*var pair = Parser.ParseKeyValue(sub[0].Label, "|");
            Address = pair.Key;
            Label = (pair.Value != "" ? pair.Value : pair.Key);

            pair = Parser.ParseKeyValue(Address, ":"); // находим в адресе NAMESPACE
            if (pair.Value != "")
            { // NAMESPACE есть
                switch (pair.Key)
                {
                    case "Файл:":
                        status = ParserStatusType.IMAGE;
                        Address = pair.Value;
                        break;
                }
            }*/
            return true;
        }

        public bool ParseLink()
        { // плоский текст
            if (status != ParserStatusType.LINK)
                return false; // пока - не положено
            if (sub == null || sub.Count == 0)
                return false;
            var pair = ParserM.ParseKeyValue(sub[0].Label, "|");
            Address = pair.Key;
            Label = (pair.Value != "" ? pair.Value : pair.Key);

            pair = ParserM.ParseKeyValue(Address, ":"); // находим в адресе NAMESPACE
            if (pair.Value != "") { // NAMESPACE есть
                switch (pair.Key){
                    case "Файл:":
                        status = ParserStatusType.IMAGE;
                        Address = pair.Value;
                        break;
                }
            }
            return true;
        }


        public void AddSub(ParsedElement newpe){
            if (newpe.Label != "") // тупо не пишем пустышки
                sub.Add(newpe);
        }

        public List<ParsedElement> ParseWikiText(string text, ref Parser p)//, ref int[] j)
        // РАЗЛОЖИТЬ чтобы оно парсило любой вики-текст
        {
            //var lst = new List<ParsedElement>();
            sub = new List<ParsedElement>();
            /*var starts = new string[] { "{{", "[[" }; // похоже строки надо константами делать
            var ends = new string[] { "}}", "]]" };   // похоже строки надо константами делать
            int len = text.Length;
            int LEL = 2;// starts.Length; // ends.Length;*/
            int cnt = 0;// 1; // счетчик открытий-закрытий
            //int js = IndexOf_M2(text, starts[0], starts[1], pos, len); // следующий открывающий
            //int je = IndexOf_M2(text, ends[0], ends[1], pos, len); // следующий закрывающий
            /*int[] j = {
                ParserM.IndexOf_M(text, starts[0], pos, len), // следующий {{
                ParserM.IndexOf_M(text, starts[1], pos, len),  // следующий [[
                ParserM.IndexOf_M(text, ends[0], pos, len),   // следующий }}
                ParserM.IndexOf_M(text, ends[1], pos, len)    // следующий ]]
            };
            int jm = j.Min(); // следующий специальный
            int ji = Array.IndexOf(j, jm);*/
            var newpe = new ParsedElement(ParserStatusType.TEXT);
            int prepos = p.pos; // индикатор, откуда считается следующий элемент 1-го уровня,
            // здесь он отстаёт от курсора при нескольких уровнях

            while (p.jm < p.len) //(js.Min() < len || je.Min() < len) // || js1 < len || je1 < len 
            {
                /*if (jd < jm && 0 == cnt)
                { // пока cnt > 0 - делить нельзя
                    lst.Add(text.Substring(pos, jd - pos));
                    pos = jd + ld;
                    jd = IndexOf_M(text, delimeter, pos, len);
                }
                else*/
                switch (p.ji)
                {
                    case 0:  // следующий {{
                        if (0 == cnt)
                        {// добавляем, только если первый уровень
                            newpe.Label = text.Substring(p.pos, p.j[0] - p.pos); // .Trim() убрать, от него пропадают пробелы между словами
                            AddSub(newpe);
                            newpe = new ParsedElement(ParserStatusType.TEMPLATE);
                            // после вместо последующей обработки пойдёт рекурсивное погружение
                            //p.pos = p.j[0] + p.LEL[0];
                            p.Recalc(0);
                            newpe.ParseWikiText(text, ref p);//, ref j); // label и текста нету, только подсписок
                            newpe.ParseTemplate();
                            AddSub(newpe);
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                            prepos = p.pos; // если новый элемент - то начало новой строки на новый курсор
                            // ВРЕМЕННО, сквозные курсоры лучше
                            /*p.j[0] = ParserM.IndexOf_M(p.text, p.s[0], p.pos, p.len);
                            p.j[1] = ParserM.IndexOf_M(p.text, p.s[1], p.pos, p.len);
                            p.j[2] = ParserM.IndexOf_M(p.text, p.s[2], p.pos, p.len);
                            p.j[3] = ParserM.IndexOf_M(p.text, p.s[3], p.pos, p.len);*/
                        }
                        else
                            p.Recalc(0);
//                            p.pos = p.j[0] + p.LEL[0];
                        //                        cnt++; теперь мы сразу разбираем внутренности, уровень не повышается
  //                      p.j[0] = ParserM.IndexOf_M(p.text, p.s[0], p.pos, p.len);
    //                    p.jm = p.j.Min();
      //                  p.ji = Array.IndexOf(p.j, p.jm);
                        break;
                    case 1:  // следующий [[
                        if (0 == cnt)
                        {// добавляем, только если первый уровень
                            newpe.Label = text.Substring(p.pos, p.j[1] - p.pos); // .Trim() убрать, от него пропадают пробелы между словами
                            AddSub(newpe);
                            newpe = new ParsedElement(ParserStatusType.LINK);
                            // после вместо последующей обработки пойдёт рекурсивное погружение
                            //p.pos = p.j[1] + p.LEL[1];
                            p.Recalc(1);
                            newpe.ParseWikiText(p.text, ref p);//, ref j); // label и текста нету, только подсписок
                            newpe.ParseLink();
                            AddSub(newpe);
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                            prepos = p.pos;  // если новый элемент - то начало новой строки на новый курсор
                            // ВРЕМЕННО, сквозные курсоры лучше
                            /*p.j[0] = ParserM.IndexOf_M(p.text, p.s[0], p.pos, p.len);
                            p.j[1] = ParserM.IndexOf_M(p.text, p.s[1], p.pos, p.len);
                            p.j[2] = ParserM.IndexOf_M(p.text, p.s[2], p.pos, p.len);
                            p.j[3] = ParserM.IndexOf_M(p.text, p.s[3], p.pos, p.len);*/
                        }
                        else
                            p.Recalc(1);
                    //  p.pos = p.j[1] + p.LEL[1];
                        //                        cnt++; теперь мы сразу разбираем внутренности, уровень не повышается
                      //  p.j[1] = ParserM.IndexOf_M(p.text, p.s[1], p.pos, p.len);
                      //  p.jm = p.j.Min();
                      //  p.ji = Array.IndexOf(p.j, p.jm);
                        break;
                    case 2:  // следующий }}
                        cnt--;
                        // добавляем новый элемент, только если ПРИШЛИ НА первый уровень
                        // а вот если НИЖЕ первого уровня - надо закрывать весь обработчик
                        if (cnt <= 0)
                        {
                            newpe.Label = text.Substring(p.pos, p.j[2] - p.pos); // .Trim() убрать, от него пропадают пробелы между словами
                            AddSub(newpe);
                            if (cnt < 0)
                            {
                                p.Recalc(2);
                                //p.pos = p.j[2] + p.LEL[2];
                                //j[2] = IndexOf_M(text, ends[0], pos, len);
                                return sub;
                            }
                            // вообще говоря, корректный уровень, только если тут TEMPLATE - но это пока не проверяем, считаем форму корректной
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                        }
                        p.Recalc(2);
//                        p.pos = p.j[2] + p.LEL[2];
  //                      p.j[2] = ParserM.IndexOf_M(p.text, p.s[2], p.pos, p.len);
    //                    p.jm = p.j.Min();
      //                  p.ji = Array.IndexOf(p.j, p.jm);
                        if (0 == cnt) prepos = p.pos;  // если новый элемент - то начало новой строки на новый курсор
                        break;
                    case 3:  // следующий ]]
                        cnt--;
                        // добавляем новый элемент, только если ПРИШЛИ НА первый уровень
                        // а вот если НИЖЕ первого уровня - надо закрывать весь обработчик
                        if (cnt <= 0)
                        {
                            newpe.Label = text.Substring(p.pos, p.j[3] - p.pos); // .Trim() убрать, от него пропадают пробелы между словами
                            AddSub(newpe);
                            if (cnt < 0)
                            {
                                p.Recalc(3);
                                //p.pos = p.j[3] + p.LEL[3];
                                //j[3] = IndexOf_M(text, ends[1], pos, len);
                                return sub;
                            }
                            // вообще говоря, корректный уровень, только если тут LINK - но это пока не проверяем, считаем форму корректной
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                        }
                        p.Recalc(3);
//                        p.pos = p.j[3] + p.LEL[3];
                        if (0 == cnt) prepos = p.pos;  // если новый элемент - то начало новой строки на новый курсор
  //                      p.j[3] = ParserM.IndexOf_M(p.text, p.s[3], p.pos, p.len);
    //                    p.jm = p.j.Min();
      //                  p.ji = Array.IndexOf(p.j, p.jm);
                        break;
                }
            }
            // когда закончили проход всего текста
            if (p.pos < p.len)
            {
                newpe.Label = text.Substring(p.pos).Trim();
                AddSub(newpe);
                p.pos = p.len;
            }
            return sub;
        }
    }

    public class Parser {
        public int[] j;
        public int[] LEL;
        public string [] s;
        public int pos;
        public string text;
        public int len;
        public int jm;
        public int ji;

        public Parser(string processedstr) {
            s = new string[] { "{{", "[[", "}}", "]]" }; // похоже строки надо константами делать
            text = processedstr;
            len = text.Length;
            LEL = new int[] { 2, 2, 2, 2 };
            j = new int[] {
                ParserM.IndexOf_M(text, s[0], pos, len), // следующий {{
                ParserM.IndexOf_M(text, s[1], pos, len),  // следующий [[
                ParserM.IndexOf_M(text, s[2], pos, len),   // следующий }}
                ParserM.IndexOf_M(text, s[3], pos, len)    // следующий ]]
            };
            jm = j.Min(); // следующий специальный
            ji = Array.IndexOf(j, jm);
        }

        public void Recalc(int i){
            pos = j[i] + LEL[i];
            j[i] = ParserM.IndexOf_M(text, s[i], pos, len);
            jm = j.Min();
            ji = Array.IndexOf(j, jm);
        }

        public List<ParsedElement> ParseWikiText(string text, ref int pos)//, ref int[] j)
        // РАЗЛОЖИТЬ чтобы оно парсило любой вики-текст
        {
            var lst = new List<ParsedElement>();
            s = new string[] { "{{", "[[", "}}", "]]" }; // похоже строки надо константами делать
            len = text.Length;
            LEL = new int[] { 2, 2, 2, 2 };
            int cnt = 0;// 1; // счетчик открытий-закрытий
            int[] j = {
                ParserM.IndexOf_M(text, s[0], pos, len), // следующий {{
                ParserM.IndexOf_M(text, s[1], pos, len),  // следующий [[
                ParserM.IndexOf_M(text, s[2], pos, len),   // следующий }}
                ParserM.IndexOf_M(text, s[3], pos, len)    // следующий ]]
            };
            int jm = j.Min(); // следующий специальный
            int ji = Array.IndexOf(j, jm);
            var newpe = new ParsedElement(ParserStatusType.TEXT);
            int prepos = pos; // индикатор, откуда считается следующий элемент 1-го уровня,
            // здесь он отстаёт от курсора при нескольких уровнях
            while (jm < len) 
            {
                switch (ji)
                {
                    case 0:  // следующий {{
                        if (0 == cnt)
                        {// добавляем, только если первый уровень
                            newpe.Label = text.Substring(pos, j[0] - pos).Trim();
                            lst.Add(newpe);
                            newpe = new ParsedElement(ParserStatusType.TEMPLATE);
                            // после вместо последующей обработки пойдёт рекурсивное погружение
                            pos = j[0] + LEL[0];
                            newpe.sub = ParseWikiText(text, ref pos);//, ref j); // label и текста нету, только подсписок
                            newpe.ParseTemplate();
                            lst.Add(newpe);
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                            prepos = pos; // если новый элемент - то начало новой строки на новый курсор
                            // ВРЕМЕННО, сквозные курсоры лучше
                            j[0] = ParserM.IndexOf_M(text, s[0], pos, len);
                            j[1] = ParserM.IndexOf_M(text, s[1], pos, len);
                            j[2] = ParserM.IndexOf_M(text, s[2], pos, len);
                            j[3] = ParserM.IndexOf_M(text, s[3], pos, len);
                        }
                        else
                            pos = j[0] + LEL[0];
                        //                        cnt++; теперь мы сразу разбираем внутренности, уровень не повышается
                        j[0] = ParserM.IndexOf_M(text, s[0], pos, len);
                        jm = j.Min();
                        ji = Array.IndexOf(j, jm);
                        break;
                    case 1:  // следующий [[
                        if (0 == cnt)
                        {// добавляем, только если первый уровень
                            newpe.Label = text.Substring(pos, j[1] - pos).Trim();
                            lst.Add(newpe);
                            newpe = new ParsedElement(ParserStatusType.LINK);
                            // после вместо последующей обработки пойдёт рекурсивное погружение
                            pos = j[1] + LEL[1];
                            newpe.sub = ParseWikiText(text, ref pos);//, ref j); // label и текста нету, только подсписок
                            newpe.ParseLink();
                            lst.Add(newpe);
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                            prepos = pos;  // если новый элемент - то начало новой строки на новый курсор
                            // ВРЕМЕННО, сквозные курсоры лучше
                            j[0] = ParserM.IndexOf_M(text, s[0], pos, len);
                            j[1] = ParserM.IndexOf_M(text, s[1], pos, len);
                            j[2] = ParserM.IndexOf_M(text, s[2], pos, len);
                            j[3] = ParserM.IndexOf_M(text, s[3], pos, len);
                        }
                        else
                            pos = j[1] + LEL[1];
                        //                        cnt++; теперь мы сразу разбираем внутренности, уровень не повышается
                        j[1] = ParserM.IndexOf_M(text, s[1], pos, len);
                        jm = j.Min();
                        ji = Array.IndexOf(j, jm);
                        break;
                    case 2:  // следующий }}
                        cnt--;
                        // добавляем новый элемент, только если ПРИШЛИ НА первый уровень
                        // а вот если НИЖЕ первого уровня - надо закрывать весь обработчик
                        if (cnt <= 0)
                        {
                            newpe.Label = text.Substring(pos, j[2] - pos).Trim();
                            lst.Add(newpe);
                            if (cnt < 0)
                            {
                                pos = j[2] + LEL[2];
                                //j[2] = IndexOf_M(text, ends[0], pos, len);
                                return lst;
                            }
                            // вообще говоря, корректный уровень, только если тут TEMPLATE - но это пока не проверяем, считаем форму корректной
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                        }
                        pos = j[2] + LEL[2];
                        if (0 == cnt) prepos = pos;  // если новый элемент - то начало новой строки на новый курсор
                        j[2] = ParserM.IndexOf_M(text, s[2], pos, len);
                        jm = j.Min();
                        ji = Array.IndexOf(j, jm);
                        break;
                    case 3:  // следующий ]]
                        cnt--;
                        // добавляем новый элемент, только если ПРИШЛИ НА первый уровень
                        // а вот если НИЖЕ первого уровня - надо закрывать весь обработчик
                        if (cnt <= 0)
                        {
                            newpe.Label = text.Substring(pos, j[3] - pos).Trim();
                            lst.Add(newpe);
                            if (cnt < 0)
                            {
                                pos = j[3] + LEL[3];
                                //j[3] = IndexOf_M(text, ends[1], pos, len);
                                return lst;
                            }
                            // вообще говоря, корректный уровень, только если тут LINK - но это пока не проверяем, считаем форму корректной
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                        }
                        pos = j[3] + LEL[3];
                        if (0 == cnt) prepos = pos;  // если новый элемент - то начало новой строки на новый курсор
                        j[3] = ParserM.IndexOf_M(text, s[3], pos, len);
                        jm = j.Min();
                        ji = Array.IndexOf(j, jm);
                        break;
                }
            }
            // когда закончили проход всего текста
            if (pos < len)
            {
                newpe.Label = text.Substring(pos).Trim();
                lst.Add(newpe);
                pos = len;
            }
            return lst;
        }
    }



    public class ParserM
    {
        static public List<ParsedElement> ParseWikiText(string text, ref int pos)//, ref int[] j)
        // РАЗЛОЖИТЬ чтобы оно парсило любой вики-текст
        {
            var lst = new List<ParsedElement>();
            var starts = new string[] { "{{", "[[" }; // похоже строки надо константами делать
            var ends = new string[] { "}}", "]]" };   // похоже строки надо константами делать
            int len = text.Length;
            int LEL = 2;// starts.Length; // ends.Length;
            int cnt = 0;// 1; // счетчик открытий-закрытий
            //int js = IndexOf_M2(text, starts[0], starts[1], pos, len); // следующий открывающий
            //int je = IndexOf_M2(text, ends[0], ends[1], pos, len); // следующий закрывающий
            int[] j = {
                IndexOf_M(text, starts[0], pos, len), // следующий {{
                IndexOf_M(text, starts[1], pos, len),  // следующий [[
                IndexOf_M(text, ends[0], pos, len),   // следующий }}
                IndexOf_M(text, ends[1], pos, len)    // следующий ]]
            };
            int jm = j.Min(); // следующий специальный
            int ji = Array.IndexOf(j, jm);
            var newpe = new ParsedElement(ParserStatusType.TEXT);
            int prepos = pos; // индикатор, откуда считается следующий элемент 1-го уровня,
            // здесь он отстаёт от курсора при нескольких уровнях

            while (jm < len) //(js.Min() < len || je.Min() < len) // || js1 < len || je1 < len 
            {
                /*if (jd < jm && 0 == cnt)
                { // пока cnt > 0 - делить нельзя
                    lst.Add(text.Substring(pos, jd - pos));
                    pos = jd + ld;
                    jd = IndexOf_M(text, delimeter, pos, len);
                }
                else*/
                switch (ji)
                {
                    case 0:  // следующий {{
                        if (0 == cnt)
                        {// добавляем, только если первый уровень
                            newpe.Label = text.Substring(pos, j[0] - pos).Trim();
                            lst.Add(newpe);
                            newpe = new ParsedElement(ParserStatusType.TEMPLATE);
                            // после вместо последующей обработки пойдёт рекурсивное погружение
                            pos = j[0] + LEL;
                            newpe.sub = ParseWikiText(text, ref pos);//, ref j); // label и текста нету, только подсписок
                            newpe.ParseTemplate();
                            lst.Add(newpe);
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                            prepos = pos; // если новый элемент - то начало новой строки на новый курсор
                            // ВРЕМЕННО, сквозные курсоры лучше
                            j[0] = IndexOf_M(text, starts[0], pos, len);
                            j[1] = IndexOf_M(text, starts[1], pos, len);
                            j[2] = IndexOf_M(text, ends[0], pos, len);
                            j[3] = IndexOf_M(text, ends[1], pos, len);
                        }
                        else
                            pos = j[0] + LEL;
//                        cnt++; теперь мы сразу разбираем внутренности, уровень не повышается
                        j[0] = IndexOf_M(text, starts[0], pos, len);
                        jm = j.Min();
                        ji = Array.IndexOf(j, jm);
                        break;
                    case 1:  // следующий [[
                        if (0 == cnt)
                        {// добавляем, только если первый уровень
                            newpe.Label = text.Substring(pos, j[1] - pos).Trim();
                            lst.Add(newpe);
                            newpe = new ParsedElement(ParserStatusType.LINK);
                            // после вместо последующей обработки пойдёт рекурсивное погружение
                            pos = j[1] + LEL;
                            newpe.sub = ParseWikiText(text, ref pos);//, ref j); // label и текста нету, только подсписок
                            newpe.ParseLink();
                            lst.Add(newpe);
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                            prepos = pos;  // если новый элемент - то начало новой строки на новый курсор
                            // ВРЕМЕННО, сквозные курсоры лучше
                            j[0] = IndexOf_M(text, starts[0], pos, len);
                            j[1] = IndexOf_M(text, starts[1], pos, len);
                            j[2] = IndexOf_M(text, ends[0], pos, len);
                            j[3] = IndexOf_M(text, ends[1], pos, len);
                        }
                        else
                            pos = j[1] + LEL;
//                        cnt++; теперь мы сразу разбираем внутренности, уровень не повышается
                        j[1] = IndexOf_M(text, starts[1], pos, len);
                        jm = j.Min();
                        ji = Array.IndexOf(j, jm);
                        break;
                    case 2:  // следующий }}
                        cnt--;
                        // добавляем новый элемент, только если ПРИШЛИ НА первый уровень
                        // а вот если НИЖЕ первого уровня - надо закрывать весь обработчик
                        if (cnt <= 0)
                        {
                            newpe.Label = text.Substring(pos, j[2] - pos).Trim();
                            lst.Add(newpe);
                            if (cnt < 0)
                            {
                                pos = j[2] + LEL;
                                //j[2] = IndexOf_M(text, ends[0], pos, len);
                                return lst;
                            }
                            // вообще говоря, корректный уровень, только если тут TEMPLATE - но это пока не проверяем, считаем форму корректной
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                        }
                        pos = j[2] + LEL;
                        if (0 == cnt) prepos = pos;  // если новый элемент - то начало новой строки на новый курсор
                        j[2] = IndexOf_M(text, ends[0], pos, len);
                        jm = j.Min();
                        ji = Array.IndexOf(j, jm);
                        break;
                    case 3:  // следующий ]]
                        cnt--;
                        // добавляем новый элемент, только если ПРИШЛИ НА первый уровень
                        // а вот если НИЖЕ первого уровня - надо закрывать весь обработчик
                        if (cnt <= 0)
                        {
                            newpe.Label = text.Substring(pos, j[3] - pos).Trim();
                            lst.Add(newpe);
                            if (cnt < 0)
                            {
                                pos = j[3] + LEL;
                                //j[3] = IndexOf_M(text, ends[1], pos, len);
                                return lst;
                            }
                            // вообще говоря, корректный уровень, только если тут LINK - но это пока не проверяем, считаем форму корректной
                            newpe = new ParsedElement(ParserStatusType.TEXT);
                        }
                        pos = j[3] + LEL;
                        if (0 == cnt) prepos = pos;  // если новый элемент - то начало новой строки на новый курсор
                        j[3] = IndexOf_M(text, ends[1], pos, len);
                        jm = j.Min();
                        ji = Array.IndexOf(j, jm);
                        break;
                }
            }
            // когда закончили проход всего текста
            if (pos < len)
            {
                newpe.Label = text.Substring(pos).Trim();
                lst.Add(newpe);
                pos = len;
            }
            return lst;
        }

        static public KeyValuePair<string, string> ParseKeyValue(string text, string delimeter)
        {
            // разбиение строки на строковую пару ключ-значение
            string key = "";
            string val = "";
            int len = text.IndexOf(delimeter);
            if (len == -1)
                return new KeyValuePair<string, string>(text, "");
            key = text.Substring(0, len).Trim();
            val = text.Substring(len + delimeter.Length).Trim();
            return new KeyValuePair<string, string>(key, val);
        }

        static public int IndexOf_M(string where, string what, int pos, int len)
        // IndexOf, возвращающий при ошибке не -1, а maxint
        {
            int i = where.IndexOf(what, pos);
            if (i < 0) i = len + 1;
            return i;
        }

        static public int IndexOf_M2(string where, string what1, string what2, int pos, int len)
        // IndexOf, возвращающий при ошибке не -1, а maxint
        // считает вхождение раннего из двух элементов    
        {
            int i1 = where.IndexOf(what1, pos);
            int i2 = where.IndexOf(what2, pos);
            if (i1 < 0) {
                if (i2 < 0)
                    return len + 1;
                else
                    return i2;
            } else if (i2 < 0)
                return i1;
            else
                return Math.Min(i1, i2);
        }

        static public List<string> ParseList(string text, string[] delimeters)
        // парсер в список, не учитывающий вложенных скобок, 
        // но позволяет разбивать по строкам.
        // используется для списка с переносом строк
        {
            // разбиение строки на список строк
            var lst = new List<string>();
            var arrs = text.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in arrs)
                lst.Add(s.Trim());
            return lst;
        }

        static public List<string> ParseList2(string text, string delimeter)//string[] delimeters)
        // парсер в список, учитывающий вложенные скобки
        // используется для разбиения шаблона по полям
        {
            var lst2 = new List<string>();
            var starts = new string[] { "{{", "[[" };
            var ends = new string[] { "}}", "]]" };
            int pos = 0;
            int len = text.Length;
            int ls = 2;// starts.Length;
            int le = 2;// ends.Length;
            int ld = delimeter.Length;// ends.Length;
            int cnt = 0;// 1; // счетчик открытий-закрытий
            int jd = IndexOf_M(text, delimeter, pos, len); // следующий разделитель
            int js = IndexOf_M2(text, starts[0], starts[1], pos, len); // следующий открывающий
            int je = IndexOf_M2(text, ends[0], ends[1], pos, len); // следующий закрывающий
            int jm = Math.Min(js, je); // следующий специальный
            int prepos = pos; // индикатор, откуда считается следующий элемент 1-го уровня,
            // здесь он отстаёт от курсора при нескольких уровнях

            //            int js0 = text.IndexOf(starts[0], pos); // следующий открывающий
            //          int je0 = text.IndexOf(ends[0], pos); // следующий закрывающий
            //        int js1 = text.IndexOf(starts[1], pos); // следующий открывающий
            //      int je1 = text.IndexOf(ends[1], pos); // следующий закрывающий
            //string res = "";
            //            while (js0 < len || je0 < len || js1 < len || je1 < len || jd < len)
            while (js < len || je < len || jd < len)
            {
                if (jd < jm && 0 == cnt)
                { // пока cnt > 0 - делить нельзя
                    lst2.Add(text.Substring(prepos, jd - prepos).Trim());
                    pos = jd + ld;
                    prepos = pos;
                    jd = IndexOf_M(text, delimeter, pos, len);
                }
                else if (js < je)
                {
                    cnt++;
                    pos = js + ls;
                    js = IndexOf_M2(text, starts[0], starts[1], pos, len);
                    jm = Math.Min(js, je);
                    jd = len + 1; // здесь ненулевой уровень, потому delimeter игнорируется
                }
                else
                { // je < js
                    cnt--;
                    pos = je + le;
                    je = IndexOf_M2(text, ends[0], ends[1], pos, len);
                    jm = Math.Min(js, je);
                    if (0 == cnt) // достижимо только на снижении cnt
                        jd = IndexOf_M(text, delimeter, pos, len); // можно пересчитать
                }
            }
            // дополняем хвост
            lst2.Add(text.Substring(prepos).Trim());
            return lst2;
        }

        static public string GetSubTemplate(string text, ref int pos, string starts, string ends)
        {
            int len = text.Length;
            int ls = starts.Length;
            int le = ends.Length;
            int cnt = 1; // счетчик открытий-закрытий
            pos = pos + ls; // текущий курсор
            int startpos = pos; // начало темплейта, именно после открывающего
            int js = text.IndexOf(starts, pos); // следующий открывающий
            int je = text.IndexOf(ends, pos); // следующий закрывающий
            if (je > 0)
            { // просто проверка, если закрывающей нет - это автоматом вылет
                string res = "";
                while (js < len || je < len) {
                    if (js < je) {
                        cnt++;
                        pos = js + ls;
                        js = IndexOf_M(text, starts, pos, len);
                    } else {
                        cnt--;
                        if (0 == cnt) // достижимо только на снижении cnt
                        { // нашли
                            res = text.Substring(startpos, je - startpos);
                            pos = je + le;
                            return res;
                        }
                        pos = je + le;
                        je = IndexOf_M(text, ends, pos, len);
                    }
                }
            }
            pos = len + 1;// -1; не сумели найти хвост, шаблона нету - пишем завершение прохождения
            return "";
        }

        static public HMElement AlphaParser(string text, string article_name, string template_name)
        // - проверяет вхождение шаблона 
        // - находит внутри шаблона ключевые слова для поиска InfoField
        // - создает список InfoField для записи в перспективе
        {
            var lst = new List<HMInfoField>();
            int len = text.Length;
            string subs = "";
            int tempcnt = 0; // считаем перебранные шаблоны, с какого-то момента считаем, что можно выкинуть
            //res = text.Contains("{{" + template_name); // примитивно
            // в полной версии template_name нам уже вероятно не нужен

            int i = IndexOf_M(text, "{{", 0, len); // считаем шаблоны только на 1-м уровне
            while (i < len)
            {
                //                if (i > 0) {
                subs = GetSubTemplate(text, ref i, "{{", "}}");
                //var infosl = ParseList(subs, new string[] { "|" }); // разбираем, находим имя типа элемента
                var infosl = ParseList2(subs, "|"); // разбираем, находим имя типа элемента
                var elt = FindElementType(infosl[0], article_name);
                // если НЕ нашли - ищем следующий шаблон
                if (null == elt)
                {
                    if (tempcnt > 3)
                        return null; // не нашли 4 шаблона - можно уже и не искать
                    tempcnt++;
                    i = IndexOf_M(text, "{{", i, len); // ищем следующий шаблон
                    continue;
                }
                // если нашли - берем тип элемента, дальше раскидываем инфо-поля из infosl
                var el = new HMElement()
                {
                    name = article_name,
                    text = text,
                    element_type = elt
                };
                el.FillElementInfo(infosl);
                if (el.PlaceElement() > 0)
                    return el; // нашли
                // потом надо подрисовать сюда создание связей                    
                //                }
            }
            return null;// не нашли
        }

        static public HMElementType FindElementType(string t_name, string a_name)
        {
            // поиск ElementType
            var lst = new List<HMElementType>();

            foreach (var elt in HMData.ElementTypes)
            {
                bool is_a_name = false;
                foreach (var s in elt.name_aliases)
                    // 1. проверять на полные слова !!! 
                    // 2. заменять предварительно _ !!!
                    if (a_name.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        is_a_name = true;
                        break;
                    }
                if (elt.template_aliases.Contains(t_name))
                    if (is_a_name)
                        lst.Add(elt);
                    else // шаблон есть, а имени нету
                        HMData.PostToLog(string.Format("a : {0} -> t : {1} : a not in {2} name_alias list", a_name, t_name, elt.name));
            }

            if (lst.Count == 1)
                return lst[0];
            else if (lst.Count > 1)
            {
                HMData.PostToLog(string.Format("a : {0} -> t : {1} : multi ElTypes : {2} {3} etc ", a_name, t_name, lst[0].name, lst[1].name));
                return lst[0];
            }
            else
                return null;
        }
    }
}
