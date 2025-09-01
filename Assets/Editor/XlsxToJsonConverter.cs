using UnityEngine;
using UnityEditor;
using System.IO;
using System.Data;
using ExcelDataReader;
using System.Text;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

public class XlsxToJsonConverter : EditorWindow
{
    private string sourceFolderPath = "Assets/Data/Xlsx";
    private string jsonOutputPath = "Assets/Resources/GeneratedJson";
    private string scriptOutputPath = "Assets/Scripts/GeneratedData";

    // Unity 에디터 상단 메뉴에 툴 등록
    [MenuItem("Tools/Data Converter/XLSX to JSON Converter")]
    public static void ShowWindow()
    {
        // GetWindow<T>를 통해 에디터 창 생성하고 보여줌
        GetWindow<XlsxToJsonConverter>("XLSX to JSON Converter");
    }

    // 에디터 윈도우의 UI 그리는 메서드
    private void OnGUI()
    {
        GUILayout.Label("XLSX to JSON Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 공통 경로 설정 UI
        // 소스, C# 스크립트, JSON 출력 폴더 경로를 설정하는 UI 필드 그리기
        EditorGUILayout.LabelField("Source Folder (XLSX)", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        sourceFolderPath = EditorGUILayout.TextField(sourceFolderPath);
        if (GUILayout.Button("Select", GUILayout.Width(60))) SelectFolder(ref sourceFolderPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Output Folder (C# Scripts)", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        scriptOutputPath = EditorGUILayout.TextField(scriptOutputPath);
        if (GUILayout.Button("Select", GUILayout.Width(60))) SelectFolder(ref scriptOutputPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Output Folder (JSON)", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        jsonOutputPath = EditorGUILayout.TextField(jsonOutputPath);
        if (GUILayout.Button("Select", GUILayout.Width(60))) SelectFolder(ref jsonOutputPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);

        // 1단계: 클래스 생성
        // HelpBox: 각 버튼이 어떤 역할을 하고 언제 사용해야 하는지 사용자에게 안내
        EditorGUILayout.HelpBox("Step 1: Generate C# script files from your Excel files. If you add a new Excel file or change its structure (add/remove columns), run this first.", MessageType.Info);
        if (GUILayout.Button("Step 1: Generate C# Classes"))
        {
            GenerateAllClasses();
        }

        EditorGUILayout.Space(10);

        // 2단계: JSON 변환
        EditorGUILayout.HelpBox("Step 2: Convert data from Excel to JSON files. Run this after Step 1 has completed and Unity has finished recompiling.", MessageType.Info);
        if (GUILayout.Button("Step 2: Convert Excel to JSON"))
        {
            ConvertAllToJson();
        }
    }

    #region --- 기능별 메서드 ---
    // 소스 폴더의 모든 엑셀 파일을 C# 클래스로 변환
    private void GenerateAllClasses()
    {
        // 폴더 존재 여부 확인
        if (!Directory.Exists(sourceFolderPath))
        {
            Debug.LogError("Source folder not found!");
            return;
        }
        
        Directory.CreateDirectory(scriptOutputPath);

        // 엑셀 파일 존재 여부 확인
        string[] excelFiles = Directory.GetFiles(sourceFolderPath, "*.xlsx", SearchOption.AllDirectories);
        // 파일 이름이 "~$"로 시작하는 임시 파일을 목록에서 제외
        excelFiles = excelFiles.Where(f => !Path.GetFileName(f).StartsWith("~$")).ToArray();

        if (excelFiles.Length == 0)
        {
            EditorUtility.DisplayDialog("No Files Found", $"No .xlsx files were found in the source folder:\n'{sourceFolderPath}'", "OK");
            Debug.LogWarning($"No .xlsx files found in {sourceFolderPath}. C# class generation stopped.");
            return; // 파일이 없음 -> 메서드 실행 중단
        }

        // 찾은 모든 엑셀 파일에 대해 반복 작업을 수행
        foreach (string filePath in excelFiles)
        {
            try
            {
                // TODO: 엑셀 파일이 열려있어도 읽을 수 있도록 처리(MemoryStream 방식 사용해도 해결 안됨)
                // 파일을 바이트 배열로 읽어 MemoryStream으로 처리
                byte[] fileBytes = File.ReadAllBytes(filePath);
                using (var stream = new MemoryStream(fileBytes))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(); // 엑셀 파일의 모든 시트를 DataSet로 읽어오기
                        foreach (DataTable table in result.Tables) // 각 시트(DataTable)에 대해 반복
                        {
                            if (table.Rows.Count < 1)
                            {
                                continue; // 비어있는 시트는 건너뜀
                            }

                            // 클래스 이름은 "시트명"으로 지정
                            string className = $"{table.TableName}";
                            GenerateCSharpClassFile(table, className, scriptOutputPath);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to read file '{filePath}'. It might be locked by another process. Error: {e.Message}");
            }
        }

        AssetDatabase.Refresh(); // 새로 생성된 C# 파일들을 Unity가 인식하도록 프로젝트 새로고침
        EditorUtility.DisplayDialog("Step 1 Complete", "C# classes generated successfully. Unity will now recompile. After that you can run Step 2.", "OK");
    }

    // 소스 폴더의 모든 엑셀 데이터를 JSON 파일로 변환
    private void ConvertAllToJson()
    {
        if (!Directory.Exists(sourceFolderPath))
        { 
            Debug.LogError("Source folder not found!");
            return; 
        }

        // JSON 변환에 필요한 C# 클래스가 모두 존재하는지 사전 확인
        if (!CheckIfAllClassesExist())
        {
            EditorUtility.DisplayDialog("Prerequisite Missing",
                "One or more required C# classes could not be found.\n\nPlease run 'Step 1: Generate C# Classes' and wait for Unity to recompile before running Step 2.",
                "OK");
            return; // 클래스가 없으므로 메서드 실행 중단
        }

        Directory.CreateDirectory(jsonOutputPath);

        string[] excelFiles = Directory.GetFiles(sourceFolderPath, "*.xlsx", SearchOption.AllDirectories);
        // 파일 이름이 "~$"로 시작하는 임시 파일을 목록에서 제외
        excelFiles = excelFiles.Where(f => !Path.GetFileName(f).StartsWith("~$")).ToArray();

        if (excelFiles.Length == 0)
        {
            EditorUtility.DisplayDialog("No Files Found", $"No .xlsx files were found in the source folder:\n'{sourceFolderPath}'", "OK");
            Debug.LogWarning($"No .xlsx files found in {sourceFolderPath}. JSON conversion stopped.");
            return; // 파일이 없음 -> 메서드 실행 중단
        }

        foreach (string filePath in excelFiles)
        {
            try
            {
                // 파일을 바이트 배열로 읽어 MemoryStream으로 처리
                byte[] fileBytes = File.ReadAllBytes(filePath);
                using (var stream = new MemoryStream(fileBytes))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // JSON 변환 시에는 첫 행을 헤더로 취급하지 않고 직접 파싱
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration { ConfigureDataTable = (_) => new ExcelDataTableConfiguration { UseHeaderRow = false } });
                        foreach (DataTable table in result.Tables)
                        {
                            if (table.Rows.Count <= 1)
                            {
                                continue; // 헤더만 있거나 빈 시트는 건너뜀
                            }

                            string className = $"{table.TableName}";
                            ConvertSheetToJson(table, className, jsonOutputPath);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to read file '{filePath}'. It might be locked by another process. Error: {e.Message}");
            }
        }

        AssetDatabase.Refresh(); // 새로 생성된 JSON 파일들을 Unity가 인식하도록 프로젝트 새로고침
        EditorUtility.DisplayDialog("Step 2 Complete", "JSON files converted successfully.", "OK");
    }
    #endregion

    #region --- 헬퍼 메서드 ---
    // 하나의 엑셀 시트(DataTable)를 분석해서 C# 클래스 파일(.cs) 생성
    private void GenerateCSharpClassFile(DataTable table, string className, string outPath)
    {
        // 첫 행은 변수명(헤더)으로 사용
        DataRow headerRow = table.Rows[0];
        // 두 번째 행은 타입 추론에 사용
        DataRow dataRowForTypeInference = table.Rows.Count > 1 ? table.Rows[1] : headerRow;

        string idFieldName = headerRow[0]?.ToString();
        // 첫 열의 이름에 'id' 또는 'key'가 포함되면 목록형(Collection) 데이터로 간주
        bool isCollection = !string.IsNullOrWhiteSpace(idFieldName) && (idFieldName.ToLower().Contains("id") || idFieldName.ToLower().Contains("key"));

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("//------------------------------------------------------------------------------");
        sb.AppendLine("// <auto-generated>");
        sb.AppendLine("//     This code was generated by a tool.");
        sb.AppendLine("//");
        sb.AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if");
        sb.AppendLine("//     the code is regenerated.");
        sb.AppendLine("// </auto-generated>");
        sb.AppendLine("//------------------------------------------------------------------------------");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("\n[Serializable]"); // Unity의 JsonUtility가 인식할 수 있도록 설정
        sb.AppendLine($"public class {className}{(isCollection ? " : IData" : "")}"); // 목록형 데이터는 IData 인터페이스 구현
        sb.AppendLine("{");

        if (isCollection)
        {
            if (InferTypeOfValue(dataRowForTypeInference[0]) != typeof(int))
            {
                Debug.LogError($"'{className}' is detected as Collection Data, but its first column '{idFieldName}' is not a number. Aborting class generation.");
                return;
            }

            // IData 인터페이스의 ID 프로퍼티 구현. 첫 열을 ID로 매핑
            sb.AppendLine($"    public int ID => {idFieldName};");
            sb.AppendLine();
        }

        // 모든 열에 대해 반복하여 멤버 변수를 생성
        for (int i = 0; i < table.Columns.Count; i++)
        {
            string fieldName = headerRow[i]?.ToString();
            if (string.IsNullOrWhiteSpace(fieldName) || fieldName.Contains(" "))
            {
                Debug.LogWarning($"Invalid field name '{fieldName}' in '{className}'. Skipping.");
                continue;
            }
            Type fieldType = InferTypeOfValue(dataRowForTypeInference[i]); // 데이터 타입 추론
            sb.AppendLine($"    public {GetTypeName(fieldType)} {fieldName};"); // "public [타입] [변수명];" 형태의 코드 추가
        }
        sb.AppendLine("}");

        if (isCollection)
        {
            // JSON 배열의 키가 될 필드 이름을 동적으로 생성
            // 예: className이 "ItemData" -> "itemdata"
            // 목록형 데이터의 경우 List를 감싸는 Table 클래스를 추가로 생성
            string listFieldName = $"{className.ToLower()}";

            sb.AppendLine($"\n[Serializable]\npublic class {className}Table\n{{");
            sb.AppendLine($"    public List<{className}> {listFieldName};");
            sb.AppendLine("}");
        }


        // 완성된 문자열을 .cs 파일로 저장
        File.WriteAllText(Path.Combine(outPath, $"{className}.cs"), sb.ToString(), Encoding.UTF8);
    }

    private void ConvertSheetToJson(DataTable table, string className, string outJsonPath)
    {
        // [리플렉션] 문자열 이름으로 C# 클래스 타입 찾음
        // 1단계에서 생성된 클래스의 Type 정보를 가져옴
        Type dataType = FindTypeInAssemblies(className);
        if (dataType == null)
        {
            Debug.LogError($"Class '{className}' not found. Please run Step 1 to generate it first.");
            return;
        }

        DataRow headerRow = table.Rows[0];
        bool isCollection = typeof(IData).IsAssignableFrom(dataType); // IData 구현 여부로 목록/단일 객체 판별
        string json;

        if (isCollection)
        {
            Type tableType = FindTypeInAssemblies($"{className}Table");
            if (tableType == null)
            { 
                Debug.LogError($"Table class for '{className}' not found."); 
                return; 
            }

            // 객체 동적 생성: Activator.CreateInstance(dataType)를 사용하여 new MyClass()를 직접 호출하지 않고도 클래스의 객체를 만듦
            // [리플렉션] new ClassNameTable()과 동일한 작업 수행
            object tableInstance = Activator.CreateInstance(tableType);
            // [리플렉션] new List<ClassName>()과 동일한 작업 수행
            object dataList = Activator.CreateInstance(typeof(List<>).MakeGenericType(dataType));

            // JSON 배열의 키가 될 필드 이름을 동적으로 찾음
            string listFieldName = $"{className.ToLower()}";
            FieldInfo listField = tableType.GetField(listFieldName);

            if (listField == null)
            {
                Debug.LogError($"Field '{listFieldName}' not found in '{tableType.Name}'. Make sure Step 1 (Class Generation) was run with the latest changes.");
                return;
            }

            // 값 할당: field.SetValue(instance, value)를 사용하여 엑셀에서 읽어온 값을 동적으로 생성한 객체의 알맞은 변수에 할당
            // [리플렉션] tableInstance.itemdatas = dataList; 와 동일한 작업 수행
            listField.SetValue(tableInstance, dataList);
            MethodInfo listAddMethod = dataList.GetType().GetMethod("Add");

            // 데이터 행들을 순회하며 리스트에 추가
            for (int i = 1; i < table.Rows.Count; i++)
            {
                DataRow dataRow = table.Rows[i];
                if (dataRow[0] == null || string.IsNullOrWhiteSpace(dataRow[0].ToString()))
                {
                    continue;
                }

                object dataInstance = Activator.CreateInstance(dataType);
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    FieldInfo field = dataType.GetField(headerRow[j].ToString());
                    if (field != null)
                    {
                        // [리플렉션] dataInstance.fieldName = (fieldType)value; 와 유사한 작업 수행
                        object value = SafeChangeType(dataRow[j], field.FieldType);
                        field.SetValue(dataInstance, value);
                    }
                }
                listAddMethod.Invoke(dataList, new[] { dataInstance }); // dataList.Add(dataInstance)와 동일
            }
            json = JsonUtility.ToJson(tableInstance, true);
        }
        else // 단일 객체 데이터
        {
            DataRow dataRow = table.Rows[1];
            object dataInstance = Activator.CreateInstance(dataType);
            for (int j = 0; j < table.Columns.Count; j++)
            {
                FieldInfo field = dataType.GetField(headerRow[j].ToString());
                if (field != null)
                {
                    object value = SafeChangeType(dataRow[j], field.FieldType);
                    field.SetValue(dataInstance, value);
                }
            }
            json = JsonUtility.ToJson(dataInstance, true);
        }

        // 완성된 JSON 문자열을 .json 파일로 저장
        File.WriteAllText(Path.Combine(outJsonPath, $"{className}.json"), json, Encoding.UTF8);
    }
    #endregion

    #region --- 유틸리티 메서드 ---
    /// <summary>
    /// 소스 폴더의 모든 엑셀 시트에 해당하는 C# 클래스가 존재하는지 확인
    /// </summary>
    /// <returns>모든 클래스가 존재하면 true 하나라도 없으면 false를 반환</returns>
    private bool CheckIfAllClassesExist()
    {
        string[] excelFiles = Directory.GetFiles(sourceFolderPath, "*.xlsx", SearchOption.AllDirectories);
        // 파일 이름이 "~$"로 시작하는 임시 파일을 목록에서 제외
        excelFiles = excelFiles.Where(f => !Path.GetFileName(f).StartsWith("~$")).ToArray();

        foreach (string filePath in excelFiles)
        {
            try
            {
                // 파일을 바이트 배열로 읽어 MemoryStream으로 처리
                byte[] fileBytes = File.ReadAllBytes(filePath);
                using (var stream = new MemoryStream(fileBytes))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        foreach (DataTable table in result.Tables)
                        {
                            if (table.Rows.Count < 1) continue;

                            string className = $"{table.TableName}";
                            Type dataType = FindTypeInAssemblies(className);

                            if (dataType == null)
                            {
                                Debug.LogError($"Prerequisite check failed: Class '{className}' is missing. Please generate it from '{filePath}' using Step 1.");
                                return false;
                            }
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to read file for class check '{filePath}'. It might be locked. Error: {e.Message}");
                // 파일을 읽을 수 없는 경우에 검사를 실패로 간주하고 중단하는 것이 안전
                return false;
            }
        }

        // 모든 루프를 통과 -> 필요한 모든 클래스가 존재
        return true;
    }

    // 폴더 선택 창을 띄우고 선택된 경로를 Unity 친화적인 상대 경로로 변환
    private void SelectFolder(ref string path)
    {
        // 기본으로 Assets 폴더가 열림
        string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");

        // 사용자가 폴더를 선택하지 않고 창을 닫았을 경우(경로가 비어있음) 아무 작업도 하지 않음
        if (!string.IsNullOrEmpty(selectedPath))
        {
            // 선택된 폴더 경로가 현재 Unity 프로젝트의 Assets 폴더 내에 있는지 확인
            // Application.dataPath는 "C:/Your/Project/Path/Assets"와 같은 절대 경로를 가리킨다
            if (selectedPath.StartsWith(Application.dataPath))
            {
                // 절대 경로를 "Assets/..." 형태의 상대 경로로 변환
                // 예: "C:/Project/Assets/Data" -> "Assets/Data"
                path = "Assets" + selectedPath.Substring(Application.dataPath.Length);
            }
            else
            {
                // Assets 폴더 바깥을 선택한 경우 사용자에게 경고 메시지를 보여줌
                Debug.LogWarning("Please select a folder within the project's Assets directory.");
            }
        }
    }

    // 셀의 값을 분석해서 가장 적절한 C# 데이터 타입 추론(int, float, bool, string)
    private Type InferTypeOfValue(object cellValue)
    {
        if (cellValue == null)
        {
            return typeof(string);
        }

        string stringValue = cellValue.ToString().Trim(); // Trim()으로 앞뒤 공백 제거

        // 'f' 접미사가 있거나 소수점이 있으면 float으로 시도
        if (stringValue.EndsWith("f", StringComparison.OrdinalIgnoreCase))
        {
            if (float.TryParse(stringValue.Substring(0, stringValue.Length - 1), out _))
            {
                return typeof(float);
            }
        }

        if (int.TryParse(stringValue, out _))
        {
            return typeof(int);
        }
        if (bool.TryParse(stringValue, out _))
        {
            return typeof(bool);
        }

        return typeof(string);
    }

    // C# 타입을 코드 생성에 사용할 문자열("int", "float" 등)로 변환
    private string GetTypeName(Type type)
    {
        if (type == typeof(int))
        {
            return "int";
        }

        if (type == typeof(float))
        {
            return "float";
        }

        if (type == typeof(bool))
        {
            return "bool";
        }

        return "string";
    }

    // 현재 프로젝트의 모든 코드(어셈블리)를 검색하여 이름이 일치하는 C# 타입 찾기
    private Type FindTypeInAssemblies(string typeName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }

    private object SafeChangeType(object value, Type conversionType)
    {
        if (value == null || value is DBNull)
        {
            // 대상 타입이 string이면 빈 문자열, 아니면 기본값(0, false 등) 반환
            return conversionType == typeof(string) ? "" : Activator.CreateInstance(conversionType);
        }

        string valueStr = value.ToString().Trim();

        try
        {
            if (conversionType == typeof(float))
            {
                // 'f' 접미사가 있다면 제거하고 float으로 변환
                if (valueStr.EndsWith("f", StringComparison.OrdinalIgnoreCase))
                {
                    return float.Parse(valueStr.Substring(0, valueStr.Length - 1));
                }
            }

            // 기본 Convert.ChangeType 시도
            return Convert.ChangeType(valueStr, conversionType);
        }
        catch (FormatException)
        {
            // 변환 실패 시 경고 로그를 남기고 기본값을 반환하여 중단 방지
            Debug.LogWarning($"Could not convert '{valueStr}' to type '{conversionType.Name}'.Using default value instead.");
            return Activator.CreateInstance(conversionType);
        }
    }
    #endregion
}
