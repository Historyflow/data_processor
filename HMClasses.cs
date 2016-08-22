using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBproc
{
    public enum HMConnectionTypes
    {
        ParentChild,    // 0
        Horisontal      // 1
    };

    public enum HMValueTypes
    {
        DataValue,    // 0
        IntValue,      // 1
        FloatValue,
        StringValue
    };

    public class HMInfoField
    {
        //public int element_id;
        public int field_id; // hm_element_type_info_fields.id

        public DateTime date_value;
        public int int_value;
        public float float_value;
        public string string_value;

        public int multivalue_col; // в случае multivalue=1 здесь хранится номер столбца
        public int target_element_id; // избыточное, для потенциальной пересылки
        public string target_element_name; // избыточное, для потенциальной пересылки

        public DateTime start_date;
        public DateTime end_date;

        //    start_date_id integer,
        //  end_date_id integer,
        public decimal trusted = 1;
        //trusted decimal DEFAULT 1,

        public string ParseInfoVal(string text)
        {
            // РАЗЛОЖИТЬ чтобы оно парсило любой вики-текст
            int pos = 0; // для прохода по тексту
//            var lst = Parser.ParseWikiText(text, ref pos, ref j);
            //var lst = ParserM.ParseWikiText(text, ref pos);
            var p = new Parser(text);
            var pel = new ParsedElement(text);
            pel.ParseWikiText(text, ref p);
            var lst = pel.sub;
            // РЫБА! 
            // сперва надо, чтобы она учитывала всю распознанную строку
            // после - поля других типов
            string res = "";
            foreach (var pel2 in lst)
                if ((pel2.status & (ParserStatusType.TEXT | ParserStatusType.LINK)) != 0)
                    res += pel2.Label;
            string_value = res;
            return string_value;
        }
    }


    public class HMElementTypeInfoField
    {
        public int id;
        public string name; // Имя из HM
        public List<StringIntItem> aliases; //набор строк соответствия из вики StringIntItem - чтобы держать там значение multivalue
        public HMValueTypes field_type;
        public int multivalue; //, -- 0 - обычное, 1 - допускает множественные значения
        //    field_order integer, 
        //    description character varying(255) DEFAULT '',
        //    weight smallint DEFAULT 5 NOT NULL
    }

    public class HMElementType
    {
        public int id;
        public string name; // Имя из HM
        public List<string> name_aliases; //набор вероятных названий статей из вики
        public List<string> template_aliases; //набор строк соответствия из вики

        public List<HMElementTypeInfoField> fields; // поля данного типа элемента 
        //    start_date_id integer,
        //  end_date_id integer,
        //trusted decimal DEFAULT 1,
    }

    public class HMElement
    {
        public int id; // при создании элемента это -1 , обновляется после добавления в БД HM
        public int WikiId; // поле для связки с Вики
        // ALTER TABLE hm_elements  ADD COLUMN wikiid integer;

        public string name; // Имя из HM, текст для URL
        public string label; // label, название, возможно дублирующее поле

        public HMElementType element_type;

        public string description; //аннотация элемента
        public string text; //текст элемента
        public string info_string; //устаревшее поле

        public List<HMInfoField> info; // поля данного элемента 

        public int weight; // DEFAULT 5

        public DateTime start_date;
        public DateTime end_date;

        // public HMShape shape // на будущее

        //  start_date_id integer,
        //  end_date_id integer,
        public decimal trusted = 1;

        public int FillElementInfo(List<string> infosl)
        {
            if (element_type == null)
                return -4;
            //HMElementType elt = element_type;
            // заполнение Element
            info = new List<HMInfoField>();
            infosl.RemoveAt(0); // удаляем заголовочную строку
            foreach (var infosls in infosl)
            { // по всему списку - каждой строке ищем соответствие в info
                var pair = ParserM.ParseKeyValue(infosls, "="); // получаем пару ключ-значение
                if (pair.Value == "")
                    continue; // пары нет, в том числе если и есть ключ
                foreach (var el_field in element_type.fields) // ищем в полях типа алиасы для ключа
                {
                    bool isf = false;
                    int multival = 0;
                    foreach (var s in el_field.aliases)
                        if (string.Compare(pair.Key, s.Name, true) == 0)
                        {
                            isf = true;
                            multival = s.Val;
                            break;  // вхождение может быть только одно
                        }
                    if (isf)
                    {// el_field найден - заполняем info
                        var fieldvals = ParserM.ParseList(pair.Value, new string[] { "<br />", "<br/>", "<br>" }); // разбираем значение построчно
                        foreach (var vals in fieldvals)
                        {
                            // разобрать строку

                            var newinfo = new HMInfoField()
                            {
                                field_id = el_field.id,
                                multivalue_col = multival
                            };
                            newinfo.ParseInfoVal(vals);
                            info.Add(newinfo);
                        }
                    }
                }
            }
            return 1;//просто код
        }

        public int PlaceElement() {
            int res = PGDB.BDInsertElement(this);
            if (res < 0)
                return -1;
            id = res;
            foreach (var eif in info) {
                int res2 = PGDB.BDInsertInfo(id, eif);
                if (res2 < 0)
                    return -2;
            }
            return res;
        }
    }


    public class HMData
    {
        public static List<HMElementType> ElementTypes = new List<HMElementType>(){
            new HMElementType(){
                id = 1,
                name = "Война",
                name_aliases = new List<string>(){
                    "война"
                },
                template_aliases = new List<string>(){
                    "Вооружённый конфликт"
                },
                fields = new List<HMElementTypeInfoField>(){
                    new HMElementTypeInfoField(){
                        id = 7,
                        name = "Стороны",
                        aliases = new List<StringIntItem>(){
                            new StringIntItem("противник1", 1),
                            new StringIntItem("противник2", 2)
                        },
                        field_type = HMValueTypes.StringValue,
                        multivalue = 1
                    },
                    new HMElementTypeInfoField(){
                        id = 9,
                        name = "Командующие",
                        aliases = new List<StringIntItem>(){
                            new StringIntItem("командир1", 1),
                            new StringIntItem("командир2", 2)
                        },
                        field_type = HMValueTypes.StringValue,
                        multivalue = 1
                    },
                    new HMElementTypeInfoField(){
                        id = 12,
                        name = "Итог",
                        aliases = new List<StringIntItem>(){
                            new StringIntItem("итог"),
                        },
                        field_type = HMValueTypes.StringValue,
                        multivalue = 0
                    }
                } 
            },        
            new HMElementType(){
                id = 2,
                name = "Сражение",
                name_aliases = new List<string>(){
                    "битва", "сражение"
                },
                template_aliases = new List<string>(){
                    "Вооружённый конфликт"
                },
                fields = new List<HMElementTypeInfoField>(){
                    new HMElementTypeInfoField(){
                        id = 1,
                        name = "Стороны",
                        aliases = new List<StringIntItem>(){
                            new StringIntItem("противник1", 1),
                            new StringIntItem("противник2", 2)
                        },
                        field_type = HMValueTypes.StringValue,
                        multivalue = 1
                    },
                    new HMElementTypeInfoField(){
                        id = 2,
                        name = "Командующие",
                        aliases = new List<StringIntItem>(){
                            new StringIntItem("командир1", 1),
                            new StringIntItem("командир2", 2)
                        },
                        field_type = HMValueTypes.StringValue,
                        multivalue = 1
                    },
                    new HMElementTypeInfoField(){
                        id = 6,
                        name = "Итог",
                        aliases = new List<StringIntItem>(){
                            new StringIntItem("итог"),
                        },
                        field_type = HMValueTypes.StringValue,
                        multivalue = 0
                    }
                } 
            }        
        };


        public static Form1 ff;

        public static void PostToLog(string msg)
        {
           // ff.tbOut.Text += "Log : " + msg + Environment.NewLine;
        }
    }

}
