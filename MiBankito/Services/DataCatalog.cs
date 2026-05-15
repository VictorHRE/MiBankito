using System.Collections.Generic;

namespace MiBankito.Services
{
    public static class DataCatalog
    {
        public static readonly string[] BankList = new[] { "BANPRO","LAFISE","BAC","FICOHSA","AVANZ","BFP" };

        // Lista completa de servicios (nombre -> código, plataforma)
        public static readonly (string name, string code, string platform)[] Services = new[]
        {
            // PAGALO TODO
            ("CREDEX DOLARES","131760317","PAGALO TODO"),
            ("ARABELA","131760386","PAGALO TODO"),
            ("EL VERDUGO","131760482","PAGALO TODO"),
            ("AVON","131760305","PAGALO TODO"),
            ("UNICOMER","131760387","PAGALO TODO"),
            ("FDL CORDOBAS","131760106","PAGALO TODO"),
            ("FDL DOLARES","131760107","PAGALO TODO"),
            ("LANCASCO","131760388","PAGALO TODO"),
            ("ENACAL","74320028","PAGALO TODO"),
            ("DISNORTE","74320034","PAGALO TODO"),
            ("CLARO","74320030","PAGALO TODO"),
            ("TIGO HOGAR","131760379","PAGALO TODO"),
            ("CASAVISION CORDOBAS","131760358","PAGALO TODO"),
            ("CASAVISION DOLARES","131760359","PAGALO TODO"),
            ("INSTACREDIT","131760076","PAGALO TODO"),
            ("LOTO","74320029","PAGALO TODO"),
            ("PAGO YOTA","131760332","PAGALO TODO"),
            ("YOTA DOLAR","131760349","PAGALO TODO"),
            ("MICREDITO DOLARES","131760371","PAGALO TODO"),
            ("EMBAJADA VISA AMERICANA","131760471","PAGALO TODO"),
            ("ELMUNDO","798126445","PAGALO TODO"),

            // PUNTO EXPRESS (colectores)
            ("CASAVISION","010702904","PUNTO EXPRESS"),
            ("CLARO","010600401","PUNTO EXPRESS"),
            ("SKY","010700501","PUNTO EXPRESS"),
            ("TELECABLE","010703101","PUNTO EXPRESS"),
            ("TIGO HOGAR","010703001","PUNTO EXPRESS"),
            ("TIGO MOVIL","010600501","PUNTO EXPRESS"),
            ("YOTA","010703201","PUNTO EXPRESS"),
            ("IBW","010703301","PUNTO EXPRESS"),
            ("DISNORTE","010100801","PUNTO EXPRESS"),
            ("DISSUR","010100801","PUNTO EXPRESS"),
            ("ENACAL","010100301","PUNTO EXPRESS"),
            ("CREDI Q INVERSIONES","010301004","PUNTO EXPRESS"),
            ("TIZO","010306101","PUNTO EXPRESS"),
            ("CREDISIMAN","010300201","PUNTO EXPRESS"),
            ("MONGE PAY","010301901","PUNTO EXPRESS"),
            ("GALLO MAS GALLO","010301902","PUNTO EXPRESS"),
            ("EL VERDUGO","010301903","PUNTO EXPRESS"),
            ("INSTACREDIT","010301801","PUNTO EXPRESS"),
            ("PROMUJER","010302101","PUNTO EXPRESS"),
            ("UNICOMER","010301601","PUNTO EXPRESS"),
            ("CREDITS","010309101","PUNTO EXPRESS"),
            ("INSS","011201401","PUNTO EXPRESS"),
            ("MAPFRE","010401201","PUNTO EXPRESS"),
            ("GLOBEX","010302001","PUNTO EXPRESS"),
            ("LANCASCO - SCENTIA","010501101","PUNTO EXPRESS"),
            ("ORIFLAME","010501201","PUNTO EXPRESS"),
            ("EMBAJADA AMERICANA","011302102","PUNTO EXPRESS"),
            ("INDRIVER","011303001","PUNTO EXPRESS"),
            ("PICAP","010300301","PUNTO EXPRESS"),
            ("PAISA","011303602","PUNTO EXPRESS"),
            ("SIERRA DE PAZ","011500101","PUNTO EXPRESS"),
        };
    }
}
