using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;

namespace QXS.ChatBot
{
    public class BotRule
    {
        protected BotRule(string Name, int Weight)
        {
            if (Name == null)
            {
                throw new ArgumentException("Name is null.", nameof(Name));
            }

            this._name = Name;
            this._weight = Weight;
        }

        protected BotRule(string Name, int Weight, Regex MessagePattern)
            : this(Name, Weight)
        {
            if (MessagePattern == null)
            {
                throw new ArgumentException("MessagePattern is null.", nameof(MessagePattern));
            }
            this._messagePattern = MessagePattern;
        }


        public BotRule(string Name, int Weight, Regex MessagePattern, Func<Match, IChatSessionInterface, string> Process)
            : this(Name, Weight, MessagePattern)
        {
            if (Process == null)
            {
                throw new ArgumentException("Process is null.", nameof(Process));
            }
            this._process = Process;
        }

        protected string _name;
        public string Name { get { return _name; } }

        protected int _weight;
        public int Weight { get { return _weight; } }

        protected Regex _messagePattern;
        public Regex MessagePattern { get { return _messagePattern; } }

        protected Func<Match, IChatSessionInterface, string> _process;

        public Func<Match, IChatSessionInterface, string> Process { get { return _process; } }

        public static BotRule CreateRuleFromXml(ChatBotRuleGenerator generator, XmlNode node)
        {
            BotRuleCodeCompiler brcc = new BotRuleCodeCompiler(node.SelectChatBotNodes("cb:Process").Cast<XmlNode>().First().InnerText);

            return new BotRule(
                generator.GetRuleName(node), 
                generator.GetRuleWeight(node), 
                new Regex(generator.GetRulePattern(node)), 
                delegate(Match match, IChatSessionInterface session) {
                    // Capture the BotRuleCodeCompiler in the lambda to execute the C# code
                    return brcc.Execute(match, session);
                } 
           );
        }
    }

}
