//
//  BundleEncode.cs
//
//  Author:
//       ${wuxingogo} <52111314ly@gmail.com>
//
//  Copyright (c) 2016 ly-user
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using UnityEngine;
using System.Collections;
using System.IO;
using SevenZip.Compression.LZMA;
using Xxtea;
using System;
using System.Security.Cryptography;
using wuxingogo.Tools;

namespace wuxingogo.bundle
{
	public class BundleEncode
	{

		public static MemoryStream DeompressAndDecryptLZMA (byte[] memory, string password)
		{
			MemoryStream output = new MemoryStream ();

			Decoder coder = new Decoder ();
			byte[] decryptedBytes = string.IsNullOrEmpty (password) ? memory : XXTEA.Decrypt (memory, password);
			using (MemoryStream mem = new MemoryStream ()) {
				using (BinaryWriter binWriter = new BinaryWriter (mem)) {
					binWriter.Write (decryptedBytes);
					mem.Position = 0;
					using (BinaryReader binReader = new BinaryReader (mem)) {
						byte[] properties = new byte[5];
						binReader.Read (properties, 0, 5);
						byte[] fileLengthBytes = new byte[8];
						binReader.Read (fileLengthBytes, 0, 8);
						long fileLength = BitConverter.ToInt64 (fileLengthBytes, 0);
						coder.SetDecoderProperties (properties);
						coder.Code (mem, output, memory.Length, fileLength, null);
					}        
				}

			}
			return output;
		}


		public static byte[] GetCompressAndEncryptLZMA (byte[] memory, string password)
		{
			byte[] value;
			using (MemoryStream inputStream = new MemoryStream (memory)) {
				Encoder coder = new Encoder ();
				using (MemoryStream compressStream = new MemoryStream ()) {
					coder.WriteCoderProperties (compressStream);
					compressStream.Write (BitConverter.GetBytes (inputStream.Length), 0, 8);
					coder.Code (inputStream, compressStream, inputStream.Length, -1, null);
					if (string.IsNullOrEmpty (password)) {
						value = compressStream.ToArray ();
					} else {
						value = XXTEA.Encrypt (compressStream.ToArray (), password);
					}
				}
			}
			return value;
		}


		public static string GetFileMD5(byte[] bytes)
		{
			byte[] md5Result;
			MD5 md5 = new MD5CryptoServiceProvider();
			md5Result = md5.ComputeHash(bytes);

			var sb = new System.Text.StringBuilder();
			for (int i = 0; i < md5Result.Length; i++)
			{
				sb.Append(md5Result[i].ToString("x2"));
			}
			return sb.ToString();

		}

		public static string GetFileMD5(string path)
		{
			byte[] md5Result;
			using (FileStream fs = new FileStream(path, FileMode.Open))
			{
				MD5 md5 = new MD5CryptoServiceProvider();
				md5Result = md5.ComputeHash(fs);
			}

			var sb = new System.Text.StringBuilder();
			for (int i = 0; i < md5Result.Length; i++)
			{
				sb.Append(md5Result[i].ToString("x2"));
			}
			return sb.ToString();

		}

		public static void CreateBinaryFile(string outputPath, byte[] value)
		{
			if( File.Exists( outputPath ) ) {
				using (MemoryStream memoryStream = new MemoryStream())
				{
					var lhs = File.ReadAllBytes (outputPath);
					memoryStream.Write (lhs, 0, lhs.Length);
					memoryStream.Write (value,0, value.Length);
					File.WriteAllBytes(outputPath, memoryStream.ToArray ());
				}
			}
			else
				File.WriteAllBytes(outputPath, value);
		}

		public static void CreateBinaryFileAndHead(string outputPath, string json)
		{
            if( File.Exists( outputPath ) )
            {
                byte[] value;
                using( MemoryStream memeory = new MemoryStream() )
                {
                    StreamUtils.Write( memeory, json );
                    value = memeory.ToArray();
                }
                using( MemoryStream memoryStream = new MemoryStream() )
                {
                    var rhs = File.ReadAllBytes( outputPath );
                    var length = BitConverter.GetBytes( value.Length );
                    memoryStream.Write( length, 0, length.Length );
                    memoryStream.Write( value, 0, value.Length );
                    memoryStream.Write( rhs, 0, rhs.Length );

                    File.Delete( outputPath );
                    File.WriteAllBytes( outputPath, memoryStream.ToArray() );
                }
            }
            else
            {
                File.WriteAllText( outputPath, json );
            }
		}
	}



}

