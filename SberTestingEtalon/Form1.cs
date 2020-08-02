using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

/**
 * @author foxety0f
 * @description Морда номер 1 для отображения конвертера
 * logtraceFilePath - путь трейса
 * logtraceFile - файл трейса
 *
 */

namespace SberTestingEtalon
{
    public partial class Form1 : Form
    {
        // Флаг выбора папки
        private Boolean isFolder = false;
        // Флаг выбора файла
        private Boolean isFile = false;
        // Паттерн для даты
        private String datePattern = "yyyy-MM-dd HH:mm:ss.f";
        // Файл логтрейса
        FileStream logtraceFile;
        // Флаг создания RAR
        private Boolean createRar = false;

        string tempDirForRar = "";

        string rarPath = "";


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Если выбран файл, то не дать выбрать папку
            if (textBox2.Text.Equals("")) { 
                //Windows выбор папки
                FolderBrowserDialog FBD = new FolderBrowserDialog();
                // Открываем диалог выбора папки
                if (FBD.ShowDialog() == DialogResult.OK)
                {
                    // Вставляем в текстбокс путь выбранной папки
                    textBox1.Text = FBD.SelectedPath;
                    // Флаг выбора папки
                    this.isFolder = true;
                    // Кнопка очистки пути, что бы можно было выбрать файл
                    button4.Visible = true;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Не дать выбрать файл если выбрана папка
            if (textBox1.Text.Equals("")) {
                //Windows выбор файла
                OpenFileDialog OFD = new OpenFileDialog();
                // Фильтр на эксельники
                OFD.Filter = "All Excel Files|*.xls;*.xlsx|Excel 2003 and lower|*.xls|Excel 2007 and high|*.xlsx;";
                // Диалог выбора файла
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    //Записываем путь к файлу
                    textBox2.Text = OFD.FileName;
                    // Ставим флаг выбора файла
                    this.isFile = true;
                    //Кнопка очистки пути, что бы можно было выбрать папку
                    button5.Visible = true;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            String now = DateTime.Now.ToString("yyyyMMdd_hhmmss");
            createRar = checkBox1.Checked;
            progressBar1.Value = 0;

            if (!isFile && !isFolder) {
                MessageBox.Show("Вы не выбрали ни папку, ни файл.");
                return;
            }

            //Чекбокс на создание RAR-файла
            if (createRar)
            {
                // Создаём путь для темповой директории
                tempDirForRar = textBox2.Text.Equals("") ? textBox1.Text + "\\" + Path.GetFileName(textBox1.Text) : Path.GetDirectoryName(textBox2.Text) + "\\" + Path.GetFileNameWithoutExtension(textBox2.Text);
                // Если темповой директории нет
                if (!Directory.Exists(tempDirForRar)){
                    // Создаём директорию
                    Directory.CreateDirectory(tempDirForRar);          
                }
                // Формируем путь для логтрейса
                string logtraceFilePath = tempDirForRar + "\\logtrace" + now.ToString() + ".txt";
                // Создаём логтрейс
                logtraceFile = File.Create(logtraceFilePath);
                writeToLogtrace("Develop by foxety0f");
                writeToLogtrace("Create File at " + now + " on path " + logtraceFilePath + "\n");
                // Формируем путь для рарника
                rarPath = textBox2.Text.Equals("") ? textBox1.Text + "\\" + Path.GetFileName(textBox1.Text) + ".rar" : Path.GetDirectoryName(textBox2.Text) + "\\" + Path.GetFileNameWithoutExtension(textBox2.Text) + ".rar";
                
            }
            // иначе, логтрейс создаём прямо в папке
            else {
                // Путь до логтрейса
                String logtraceFilePath = textBox2.Text.Equals("") ? textBox1.Text + "\\" + "logtrace" + now.ToString() + ".txt" : Path.GetDirectoryName(textBox2.Text) + "\\" + "logtrace" + now.ToString() + ".txt";
                // Файл логтрейса
                logtraceFile = File.Create(logtraceFilePath);
                writeToLogtrace("Develop by foxety0f");
                writeToLogtrace("Create File at " + now + " on path " + logtraceFilePath + "\n");
            }

            // Если выбрана папка
            if (isFolder)
            {
                writeToLogtrace("Selected Folder");
                // Получаем все файлики .xls*, сюда входят xlsx + xlsm
                string[] filesPath = Directory.GetFiles(textBox1.Text, "*.xls");

                // Каждый файлик прогоняем через метод подготовки эталонов
                foreach (string filePath in filesPath) {
                    TestsEtalons(filePath, Path.GetFileName(filePath));
                }
            }
            // Если выбран файл
            else if(isFile) {
                writeToLogtrace("Selected File");
                // Прогоняем файл через метод подготовки эталонов
                TestsEtalons(textBox2.Text, Path.GetFileName(textBox2.Text));
            }


            if (createRar)
            {
                writeToLogtrace("\n");
                writeToLogtrace("***************RAR CREATE*****************");
                writeToLogtrace("Check exist file");
                // Если существует файлик, то удалить
                if (File.Exists(rarPath)) {
                    writeToLogtrace("Delete file with same name");
                    File.Delete(rarPath);
                }

                // Создаём архив
                using (var archive = System.IO.Compression.ZipFile.Open(rarPath, ZipArchiveMode.Create)) {
                    writeToLogtrace("Write from temp source folder");
                    // Получаем все файлы в темповой таблице
                    string[] filesToArchive = Directory.GetFiles(tempDirForRar);

                    // Каждый файлик кладём из темповой в архив
                    foreach (string flarch in filesToArchive) {
                        writeToLogtrace("Append file to rar " + Path.GetFileName(flarch));

                        // Пропускаем логтрейс
                        if(!Path.GetExtension(flarch).Equals(".txt"))
                            archive.CreateEntryFromFile(flarch, Path.GetFileName(flarch));
                    }
                }


                // Получаем список темповых файлов
                string[] filesForDelete = Directory.GetFiles(tempDirForRar);
                writeToLogtrace("Delete temp files");
                writeToLogtrace("Delete temp directory");
                // Удаляем каждый файл
                foreach (string fldel in filesForDelete){

                    // До удаления закрываем логтрейс и кладём в архив
                    if (Path.GetExtension(fldel).Equals(".txt"))
                    {
                        logtraceFile.Close();
                        using (var archive = System.IO.Compression.ZipFile.Open(rarPath, ZipArchiveMode.Update))
                        {
                            archive.CreateEntryFromFile(fldel, Path.GetFileName(fldel));
                        }
                        
                    }
                    // Удаляем текущий файл
                    File.Delete(fldel);
                }
                // Удаляем папку
                Directory.Delete(tempDirForRar);
            }
            // Закрываем ещё раз, потому что может быть не архив
            logtraceFile.Close();


            progressBar1.Value = 100;
            MessageBox.Show("Converting is end", "Done");
        }

        /**
         * @author foxety0f
         * @description метод подготовки эталонов
         * 
         * */
        private void TestsEtalons(String filePath, String fileName) {

            // Фиксируем время для того, что бы зафиксировать время исполнения
            DateTime nowCsv = DateTime.Now;
            String csv = "";
            writeToLogtrace("**********************************************************************");
            writeToLogtrace("Read file " + fileName + " on path : " + Path.GetDirectoryName(filePath));

            // Берём книгу
            IWorkbook workbook;
            // Читаем из книги
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {

                // Думаем, что это xlsx
                workbook = new XSSFWorkbook(fs);

                
                // Берём самый последний шит
                ISheet sheet = workbook.GetSheetAt(workbook.NumberOfSheets - 1);

                
                // Если на нём есть данные
                if (sheet != null) {
                    // Количество строк
                    int rowCount = sheet.LastRowNum;
                    writeToLogtrace("Number of rows : " + (rowCount + 1));

                    // Количество столбцов в первой строке
                    int cellCount = sheet.GetRow(0).LastCellNum;

                    writeToLogtrace("Number of cells : " + (cellCount + 1));
                    for (int i = 0; i <= rowCount; i++) {
                        // Берём i-тую строчку
                        IRow row = sheet.GetRow(i);

                        for (var j = 0; j < cellCount; j++) {

                            // Что бы не влепить ; перед первым значением в столбце
                            if (j != 0) {
                                csv += textBox3.Text == "" ? ";" : textBox3.Text;
                            }

                            // трай маст дай
                            try
                            {
                                
                                String valCell = "";
                                // Если первая строчка, то в ловеркейс
                                valCell = i == 0 ? GetStringValue(row.GetCell(j)).ToLower() : GetStringValue(row.GetCell(j));

                                // если первая строчка, ищем точки, если есть, берём всё, что после неё
                                if (i == 0) {
                                    if (valCell.IndexOf(".") > 0) {
                                        valCell = valCell.Substring(valCell.IndexOf(".") + 1);
                                    }

                                }

                                // добавляем значение в csv с реплейсом #

                                if (valCell.IndexOf(";") > 0) {
                                    valCell = (textBox4.Text == "" ? "\"" : textBox4.Text) + valCell + (textBox4.Text == "" ? "\"" : textBox4.Text);
                                }

                                csv += i == 0 ? valCell.Replace("#", "_") : valCell;

                                

                            }
                            // никогда не повредит
                            catch (System.NullReferenceException) {
                                csv += "NULL";
                            }
                        }
                        // UNIX-формат сразу. NOTIFY : \r\n - windows, \n - UNIX
                        csv += "\n";
                    }
                }
            }


            // наименование файла в нижний регистр
            String csvName = Path.GetFileNameWithoutExtension(fileName).ToLower();

            // избавляемся от схемы
            int indexOfDot = csvName.IndexOf(".");

            if (indexOfDot > 0) {
                csvName = csvName.Substring(indexOfDot + 1);
            }

            // Если не rar-файл
            if (!createRar)
            {
                // создаём файл csv
                using (FileStream csvFile = File.Create(Path.GetDirectoryName(filePath) + "\\" + csvName + ".csv"))
                {
                    // Энкодим в UTF-8
                    byte[] csvByte = new UTF8Encoding(true).GetBytes(csv);
                    // Пишем байты по длинне
                    csvFile.Write(csvByte, 0, csvByte.Length);
                    // Закрываем файл
                    csvFile.Close();
                }
            }
            else {
                // Если rar, то просто другой путь (мб я аут, но я усталь)
                using (FileStream csvFile = File.Create(tempDirForRar + "\\" + csvName + ".csv"))
                {
                    byte[] csvByte = new UTF8Encoding(true).GetBytes(csv);
                    csvFile.Write(csvByte, 0, csvByte.Length);
                    csvFile.Close();
                }
            }
            writeToLogtrace("Time to create - " + DateTime.Now.Subtract(nowCsv).TotalSeconds + "s");
        }

        // кнопа сброса пути
        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            button4.Visible = false;
        }

        // кнопка сброса пути
        private void button5_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            button5.Visible = false;
        }

        // для энкодинга значения
        public string GetStringValue(ICell cell)
        {
            // если хз что за ячейка, верни null
            if (cell == null) {
                return "NULL";
            }

            // поехали по ячейкам
            switch (cell.CellType)
            {
                // если нумбер
                case CellType.Numeric:
                    // смотрим, подходит ли число под дату
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        // трай маст дай
                        try
                        {
                            // Приводим дату по паттерну
                            return cell.DateCellValue.ToString(datePattern);
                        }
                        // Не помешает
                        catch (NullReferenceException)
                        {
                            // Приводим дату к эквиваленту OLE
                            return DateTime.FromOADate(cell.NumericCellValue).ToString(datePattern);
                        }
                    }
                    // Если не дата, ретурним число
                    return cell.NumericCellValue.ToString();

                case CellType.String:
                    return cell.StringCellValue;

                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();

                default:
                    return string.Empty;
            }
        }

        // Пишем в лог
        public void writeToLogtrace(string val) {

                // Превращаем в байты
            byte[] byteArray = new UTF8Encoding(true).GetBytes(val + "\n");

            // Пишем в файл
            logtraceFile.Write(byteArray, 0, byteArray.Length);
            label3.Text = val;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
