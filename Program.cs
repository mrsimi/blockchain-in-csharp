using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace blockchain_
{
    class Program
    {
        static void Main(string[] args)
        {
            Blockchain matthCoin = new Blockchain();
            matthCoin.addBlock(new Block(1, "01/02/2017", new Dictionary<string, string>() { {"amount_sent", "50 000" }}));
            matthCoin.addBlock(new Block(2, "01/03/2017", new Dictionary<string, string>() { {"amount_sent", "45 0000"}}));
            
            var objSerialize = JsonConvert.SerializeObject(matthCoin, Formatting.Indented);
           Console.WriteLine(objSerialize);           
            Console.ReadLine();
        }
    }
    class Block
    {
        public int index;
        public string timeStamp;
        public IDictionary<string, string> data;
        public string previousHash;
        public string hash;

        public Block(int index, string timeStamp, IDictionary<string, string> data, string previousHash =" ")
        {
            this.index = index;
            this.timeStamp = timeStamp;
            this.data = data;
            this.previousHash = previousHash;
            this.hash = calculateHash();
        }

        public string calculateHash()
        {
            return ComputeSha256Hash(index.ToString() + timeStamp.ToString() + data.ToString() + previousHash.ToString());
        }


        static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }


    class Blockchain 
    {
        public List<Block> chain = new List<Block>();

        public Blockchain()
        {
            this.chain.Add(createGeneisBlock());
        }

        private Block createGeneisBlock()
        {
            return new Block(0, "01/01/2017", 
                new Dictionary<string, string>() { { "amount", "4" } }, "0");
        }

        private Block getLatestBlock()
        {
            return this.chain.ElementAt(this.chain.Count - 1);
        }

        public void addBlock(Block newBlock)
        {
            newBlock.previousHash = this.getLatestBlock().hash;
            newBlock.hash = newBlock.calculateHash();
            this.chain.Add(newBlock);
        }

        public bool isChainValid()
        {
            for (int i = 1; i < this.chain.Count; i++)
            {
                Block currentBlock = this.chain.ElementAt(i);
                Block previousBlock = this.chain.ElementAt(i - 1);

                if (currentBlock.hash != currentBlock.calculateHash())
                {
                    return false;
                }
                if (currentBlock.previousHash != previousBlock.hash)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
