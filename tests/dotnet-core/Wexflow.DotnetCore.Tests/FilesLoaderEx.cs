using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wexflow.DotnetCore.Tests
{
    [TestClass]
    public class FilesLoaderEx
    {
        public static readonly string SourceFilesFolder =
            Path.Combine(Helper.SourceFilesFolder, "FilesLoaderEx") + Path.DirectorySeparatorChar;

        private static readonly string ExpectedResult138AddMaxCreateDate =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"+
            "<WexflowProcessing>\r\n"+
            "  <Workflow id=\"142\" name=\"Workflow_FilesLoaderEx_AddMaxCreateDate\" description=\"Workflow_FilesLoaderEx_AddMaxCreateDate\">\r\n"+
            "    <Files>\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file4.txt\" name=\"file4.txt\" renameTo=\"\" renameToOrName=\"file4.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file5.txt\" name=\"file5.txt\" renameTo=\"\" renameToOrName=\"file5.txt\" />\r\n"+
            "    </Files>\r\n"+
            "  </Workflow>\r\n"+
            "</WexflowProcessing>";

        private static readonly string ExpectedResult139AddMinCreateDate =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"+
            "<WexflowProcessing>\r\n"+
            "  <Workflow id=\"143\" name=\"Workflow_FilesLoaderEx_AddMinCreateDate\" description=\"Workflow_FilesLoaderEx_AddMinCreateDate\">\r\n"+
            "    <Files>\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file1.txt\" name=\"file1.txt\" renameTo=\"\" renameToOrName=\"file1.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file2.txt\" name=\"file2.txt\" renameTo=\"\" renameToOrName=\"file2.txt\" />\r\n"+
            "    </Files>\r\n"+
            "  </Workflow>\r\n"+
            "</WexflowProcessing>";

        private static readonly string ExpectedResult140AddMaxModifyDate =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"+
            "<WexflowProcessing>\r\n"+
            "  <Workflow id=\"144\" name=\"Workflow_FilesLoaderEx_AddMaxModifyDate\" description=\"Workflow_FilesLoaderEx_AddMaxModifyDate\">\r\n"+
            "    <Files>\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file4.txt\" name=\"file4.txt\" renameTo=\"\" renameToOrName=\"file4.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file5.txt\" name=\"file5.txt\" renameTo=\"\" renameToOrName=\"file5.txt\" />\r\n"+
            "    </Files>\r\n"+
            "  </Workflow>\r\n"+
            "</WexflowProcessing>";

        private static readonly string ExpectedResult141AddMinModifyDate =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"+
            "<WexflowProcessing>\r\n"+
            "  <Workflow id=\"145\" name=\"Workflow_FilesLoaderEx_AddMinModifyDate\" description=\"Workflow_FilesLoaderEx_AddMinModifyDate\">\r\n"+
            "    <Files>\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file1.txt\" name=\"file1.txt\" renameTo=\"\" renameToOrName=\"file1.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file2.txt\" name=\"file2.txt\" renameTo=\"\" renameToOrName=\"file2.txt\" />\r\n"+
            "    </Files>\r\n"+
            "  </Workflow>\r\n"+
            "</WexflowProcessing>";

        private static readonly string ExpectedResult142RemoveMaxCreateDate =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"+
            "<WexflowProcessing>\r\n"+
            "  <Workflow id=\"146\" name=\"Workflow_FilesLoaderEx_RemoveMaxCreateDate\" description=\"Workflow_FilesLoaderEx_RemoveMaxCreateDate\">\r\n"+
            "    <Files>\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file1.txt\" name=\"file1.txt\" renameTo=\"\" renameToOrName=\"file1.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file2.txt\" name=\"file2.txt\" renameTo=\"\" renameToOrName=\"file2.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file3.txt\" name=\"file3.txt\" renameTo=\"\" renameToOrName=\"file3.txt\" />\r\n"+
            "    </Files>\r\n"+
            "  </Workflow>\r\n"+
            "</WexflowProcessing>";

        private static readonly string ExpectedResult143RemoveMinCreateDate =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"+
            "<WexflowProcessing>\r\n"+
            "  <Workflow id=\"147\" name=\"Workflow_FilesLoaderEx_RemoveMinCreateDate\" description=\"Workflow_FilesLoaderEx_RemoveMinCreateDate\">\r\n"+
            "    <Files>\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file3.txt\" name=\"file3.txt\" renameTo=\"\" renameToOrName=\"file3.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file4.txt\" name=\"file4.txt\" renameTo=\"\" renameToOrName=\"file4.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file5.txt\" name=\"file5.txt\" renameTo=\"\" renameToOrName=\"file5.txt\" />\r\n"+
            "    </Files>\r\n"+
            "  </Workflow>\r\n"+
            "</WexflowProcessing>";

        private static readonly string ExpectedResult144RemoveMaxModifyDate =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"+
            "<WexflowProcessing>\r\n"+
            "  <Workflow id=\"148\" name=\"Workflow_FilesLoaderEx_RemoveMaxModifyDate\" description=\"Workflow_FilesLoaderEx_RemoveMaxModifyDate\">\r\n"+
            "    <Files>\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file1.txt\" name=\"file1.txt\" renameTo=\"\" renameToOrName=\"file1.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file2.txt\" name=\"file2.txt\" renameTo=\"\" renameToOrName=\"file2.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file3.txt\" name=\"file3.txt\" renameTo=\"\" renameToOrName=\"file3.txt\" />\r\n"+
            "    </Files>\r\n"+
            "  </Workflow>\r\n"+
            "</WexflowProcessing>";

        private static readonly string ExpectedResult145RemoveMinModifyDate =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"+
            "<WexflowProcessing>\r\n"+
            "  <Workflow id=\"149\" name=\"Workflow_FilesLoaderEx_RemoveMinModifyDate\" description=\"Workflow_FilesLoaderEx_RemoveMinModifyDate\">\r\n"+
            "    <Files>\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file3.txt\" name=\"file3.txt\" renameTo=\"\" renameToOrName=\"file3.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file4.txt\" name=\"file4.txt\" renameTo=\"\" renameToOrName=\"file4.txt\" />\r\n"+
            "      <File taskId=\"1\" path=\"" + SourceFilesFolder + "file5.txt\" name=\"file5.txt\" renameTo=\"\" renameToOrName=\"file5.txt\" />\r\n"+
            "    </Files>\r\n"+
            "  </Workflow>\r\n"+
            "</WexflowProcessing>";

        public void TestInitialize(int workflowId)
        {
            var tempFolder = Path.Combine(Helper.TempFolder, workflowId.ToString());
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);
            Helper.DeleteFilesAndFolders(tempFolder);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        public void Execute(int workflowId, string expectedResult)
        {
            TestInitialize(workflowId);
            Helper.StartWorkflow(workflowId);

            // Check the workflow result
            string[] files = Directory.GetFiles(
                Path.Combine(Helper.TempFolder, workflowId.ToString()),
                "ListFiles*.xml",
                SearchOption.AllDirectories);
            Assert.AreEqual(1, files.Length);

            string content = File.ReadAllText(files[0]);
            Assert.AreEqual(expectedResult, content);
        }

        [TestMethod]
        public void FilesLoaderExTest_142AddMaxCreateDate() => Execute(142, ExpectedResult138AddMaxCreateDate);

        [TestMethod]
        public void FilesLoaderExTest_143AddMinCreateDate() => Execute(143, ExpectedResult139AddMinCreateDate);

        [TestMethod]
        public void FilesLoaderExTest_144AddMaxModifyDate() => Execute(144, ExpectedResult140AddMaxModifyDate);

        [TestMethod]
        public void FilesLoaderExTest_145AddMinModifyDate() => Execute(145, ExpectedResult141AddMinModifyDate);

        [TestMethod]
        public void FilesLoaderExTest_146RemoveMaxCreateDate() => Execute(146, ExpectedResult142RemoveMaxCreateDate);

        [TestMethod]
        public void FilesLoaderExTest_147RemoveMinCreateDate() => Execute(147, ExpectedResult143RemoveMinCreateDate);

        [TestMethod]
        public void FilesLoaderExTest_148RemoveMaxModifyDate() => Execute(148, ExpectedResult144RemoveMaxModifyDate);

        [TestMethod]
        public void FilesLoaderExTest_149RemoveMinModifyDate() => Execute(149, ExpectedResult145RemoveMinModifyDate);
    }
}
