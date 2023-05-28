package com.chandravir.quiz

object Constants {

    const val USER_NAME: String = "user_name"
    const val TOTAL_QUESTIONS: String = "total_question"
    const val CORRECT_ANSWERS: String = "correct_answers"

    fun getQuestions(): ArrayList<Question>{
        val questionsList = ArrayList<Question>()

            val ques1 = Question(1,
                "What is the synonym of ACCORD?",
            "Pardon",
                "Forgive",
                "Agreement",
                "Reliable",
                3,
            )

        questionsList.add(ques1)

        val ques2 = Question(2,
            "What is the synonym of APATHY?",
            "Indifference",
            "Harmony",
            "Outsider",
            "Calamity",
            2
        )

        questionsList.add(ques2)

        val ques3 = Question(3,
            "What is the synonym of ALIEN?",
            "Outsider",
            "Renounce",
            "Provoke",
            "Misfortune",
            1,
        )

        questionsList.add(ques3)

        val ques4 = Question(4,
            "What is the synonym of BLEAK?",
            "Vain",
            "Dismal",
            "Deny",
            "Sworn",
            2,
        )

        questionsList.add(ques4)

        val ques5 = Question(5,
            "What is the synonym of CONCEDE?",
            "Kindness",
            "Rough",
            "Yield",
            "Include",
            3,
        )

        questionsList.add(ques5)

        val ques6 = Question(6,"What is the synonym of ABOUT ?","genius","favour","approximately","even out",3)
        questionsList.add(ques6)

        val ques7 = Question(7,"What is the synonym of ABROAD ?","overseas","fanny","downcast","buns",1)
        questionsList.add(ques7)

        val ques8 = Question(8,"What is the synonym of ABOVE ?","development","blanket","to a higher place","yoke",3)
        questionsList.add(ques8)

        val ques9 = Question(9,"What is the synonym of ABSOLUTE ?","pot","IN","office","downright",4)
        questionsList.add(ques9)

        val ques10 = Question(10,"What is the synonym of ACADEMIC ?","fragile","petty","doomed","academician",4)
        questionsList.add(ques10)

        val ques11 = Question(11,"What is the synonym of ACCORDING ?","rack","offer","harmonize","slip",3)
        questionsList.add(ques11)

        val ques12 = Question(12,"What is the synonym of ACCURATE ?","exact","gelt","bash","fanny",1)
        questionsList.add(ques12)

        val ques13 = Question(13,"What is the synonym of ACID ?","mouth","go forth","back breaker","sit",3)
        questionsList.add(ques13)

        val ques14 = Question(14,"What is the synonym of ACTIVE ?","shorten","dynamic","flake","flick",2)
        questionsList.add(ques14)

        val ques15 = Question(15,"What is the synonym of ACTIVIST ?","militant","core","equal","fragile",1)
        questionsList.add(ques15)

        val ques16 = Question(16,"What is the synonym of ACTUAL ?","existent","change","make","leaning",1)
        questionsList.add(ques16)

        val ques17 = Question(17,"What is the synonym of ADDITIONAL ?","assail","extra","check","lot",2)
        questionsList.add(ques17)

        val ques18 = Question(18,"What is the synonym of ADEQUATE ?","mean","fundament","damned","equal",4)
        questionsList.add(ques18)

        val ques19 = Question(19,"What is the synonym of ADOLESCENT ?","just about","comprehend","family relationship","stripling",4)
        questionsList.add(ques19)

        val ques20 = Question(20,"What is the synonym of ADULT ?","grownup","restore","reasonably","safe",1)
        questionsList.add(ques20)

        val ques21 = Question(21,"What is the synonym of ADVANCE ?","progress","incorrect","reign","loss",1)
        questionsList.add(ques21)

        val ques22 = Question(22,"What is the synonym of AFTER ?","drop dead","tip","later","aroused",3)
        questionsList.add(ques22)

        val ques23 = Question(23,"What is the synonym of AGGRESSIVE ?","cut off","fast-growing","religion","strain",2)
        questionsList.add(ques23)

        val ques24 = Question(24,"What is the synonym of AGO ?","agone","nonplus","wear out","bind",1)
        questionsList.add(ques24)

        val ques25 = Question(25,"What is the synonym of AGRICULTURAL ?","impression","agrarian","grass","feel",2)
        questionsList.add(ques25)

        val ques26 = Question(26,"What is the synonym of AHEAD ?","hold up","quit","in the lead","fright",3)
        questionsList.add(ques26)

        val ques27 = Question(27,"What is the synonym of ALL ?","involve","aid","wholly","insure",3)
        questionsList.add(ques27)

        val ques28 = Question(28,"What is the synonym of ALONE ?","see","movement","lone","shop",3)
        questionsList.add(ques28)

        val ques29 = Question(29,"What is the synonym of ALTERNATIVE ?","corporate","most","have","option",4)
        questionsList.add(ques29)

        val ques30 = Question(30,"What is the synonym of AMAZING ?","amaze","down","carry on","bear",1)
        questionsList.add(ques30)

        val ques31 = Question(31,"What is the synonym of ANCIENT ?","take","pricey","antediluvian","ravish",3)
        questionsList.add(ques31)

        val ques32 = Question(32,"What is the synonym of ANGRY ?","furious","pass along","call","sise",1)
        questionsList.add(ques32)

        val ques33 = Question(33,"What is the synonym of ANIMAL ?","block","animate being","feeling","program",2)
        questionsList.add(ques33)

        val ques34 = Question(34,"What is the synonym of ANNUAL ?","use","yearly","involve","heap",2)
        questionsList.add(ques34)

        val ques35 = Question(35,"What is the synonym of ANOTHER ?","some other","yobbo","gild","shed light on",1)
        questionsList.add(ques35)

        val ques36 = Question(36,"What is the synonym of ANY ?","hinder","cipher","flat","whatever",4)
        questionsList.add(ques36)

        val ques37 = Question(37,"What is the synonym of APART ?","isolated","case","scene","raise",1)
        questionsList.add(ques37)

        val ques38 = Question(38,"What is the synonym of APPARENT ?","measure","evident","hind","get",2)
        questionsList.add(ques38)

        val ques39 = Question(39,"What is the synonym of APPROPRIATE ?","allow","treat","braggy","priming",1)
        questionsList.add(ques39)

        val ques40 = Question(40,"What is the synonym of ARMED ?","morn","arm","honk","equalise",2)
        questionsList.add(ques40)

        val ques41 = Question(41,"What is the synonym of ARTISTIC ?","lot","arise","support","aesthetic",4)
        questionsList.add(ques41)

        val ques42 = Question(42,"What is the synonym of ASLEEP ?","reveal","benumbed","sham","zip",2)
        questionsList.add(ques42)

        val ques43 = Question(43,"What is the synonym of ASSISTANT ?","occupy","over","colly","helper",4)
        questionsList.add(ques43)

        val ques44 = Question(44,"What is the synonym of ASSOCIATE ?","companion","shite","waggle","grapple",1)
        questionsList.add(ques44)

        val ques45 = Question(45,"What is the synonym of ATHLETIC ?","end up","call","acrobatic","sooty",3)
        questionsList.add(ques45)

        val ques46 = Question(46,"What is the synonym of AVAILABLE ?","delegate","give","useable","trance",3)
        questionsList.add(ques46)

        val ques47 = Question(47,"What is the synonym of AVERAGE ?","berth","shite","resolve","norm",4)
        questionsList.add(ques47)

        val ques48 = Question(48,"What is the synonym of AWARE ?","cognizant","he","cast-iron","budge",1)
        questionsList.add(ques48)

        val ques49 = Question(49,"What is the synonym of AWAY ?","repair","outside","lift","cast",2)
        questionsList.add(ques49)

        val ques50 = Question(50,"What is the synonym of AWFUL ?","neural","atrocious","form","get ahead",2)
        questionsList.add(ques50)

        val ques51 = Question(	51,	"What is the synonym of BACK ?",	"train",	"Solanum tuberosum",	"vertebral column",	"bloom",	3
        )
        questionsList.add(ques51)

        val ques52 = Question(	52,	"What is the synonym of BAD ?",	"all the same",	"high-risk",	"have it off",	"send on",	2
        )
        questionsList.add(ques52)

        val ques53 = Question(	53,	"What is the synonym of BASE ?",	"likely",	"arouse",	"set up",	"fundament",	4
        )
        questionsList.add(ques53)

        val ques54 = Question(	54,	"What is the synonym of BEAT ?",	"die hard",	"goggle box",	"metre",	"engage",	3
        )
        questionsList.add(ques54)

        val ques55 = Question(	55,	"What is the synonym of BEGINNING ?",	"first",	"organise",	"finale",	"lead by the nose",	1
        )
        questionsList.add(ques55)

        val ques56 = Question(	56,	"What is the synonym of BEHIND ?",	"typeface",	"skunk",	"running play",	"nates",	4
        )
        questionsList.add(ques56)

        val ques57 = Question(	57,"What is the synonym of BEST ?","lastly","extension",	"outdo",	"order",	3
        )
        questionsList.add(ques57)

        val ques58 = Question(	58,	"What is the synonym of BETTER ?",	"bettor",	"release",	"expunge",	"mysterious",	1
        )
        questionsList.add(ques58)

        val ques59 = Question(	59,	"What is the synonym of BIG ?",	"finale",	"inducement",	"endeavor",	"large",	4
        )
        questionsList.add(ques59)

        val ques60 = Question(	60,	"What is the synonym of BILLION ?",	"fox",	"brand",	"commingle",	"one million million",	4
        )
        questionsList.add(ques60)

        val ques61 = Question(	61,	"What is the synonym of BIOLOGICAL ?",	"gruelling",	"suspensor",	"biologic",	"nasty",	3
        )
        questionsList.add(ques61)

        val ques62 = Question(	62,	"What is the synonym of BLACK ?",	"consecrate",	"appurtenance",	"hurry",	"blackness",	4
        )
        questionsList.add(ques62)

        val ques63 = Question(	63,	"What is the synonym of BLAME ?",	"blasted",	"congenator",	"exploited",	"charabanc",	1
        )
        questionsList.add(ques63)

        val ques64 = Question(	64,	"What is the synonym of BLANKET ?",	"all-embracing",	"Grass",	"serenity",	"range",	1
        )
        questionsList.add(ques64)

        val ques65 = Question(	65,	"What is the synonym of BLIND ?",	"say",	"photograph",	"dim",	"brass",	3
        )
        questionsList.add(ques65)

        val ques66 = Question(	66,	"What is the synonym of BLUE ?",	"boost",	"telegraph",	"leave out",	"blue air",	4
        )
        questionsList.add(ques66)

        val ques67 = Question(	67,	"What is the synonym of BONE ?",	"earmark",	"train",	"postal service",	"cram",	4
        )
        questionsList.add(ques67)

        val ques68 = Question(	68,	"What is the synonym of BORN ?",	"line of descent",	"intent",	"Max Born",	"extensive",	3
        )
        questionsList.add(ques68)

        val ques69 = Question(	69,	"What is the synonym of BOSS ?",	"chief",	"give",	"Elmer Rice",	"discover",	1
        )
        questionsList.add(ques69)

        val ques70 = Question(	70,	"What is the synonym of BOTTOM ?",	"take chances",	"underside",	"evaporate",	"proceeds",	2
        )
        questionsList.add(ques70)

        val ques71 = Question(	71,	"What is the synonym of BRIEF ?",	"orchard apple tree",	"legal brief",	"disturbed",	"undefendable",	2
        )
        questionsList.add(ques71)

        val ques72 = Question(	72,	"What is the synonym of BRIGHT ?",	"solvent",	"occult",	"vociferation",	"brilliant",	4
        )
        questionsList.add(ques72)

        val ques73 = Question(	73,	"What is the synonym of BRILLIANT ?",	"nurture",	"range",	"magnificent",	"intelligence operation",	3
        )
        questionsList.add(ques73)

        val ques74 = Question(	74,	"What is the synonym of BROAD ?",	"run into",	"lean",	"term",	"wide",	4
        )
        questionsList.add(ques74)

        val ques75 = Question(	75,	"What is the synonym of BROKEN ?",	"interrupt",	"surely",	"meander",	"capable",	1
        )
        questionsList.add(ques75)

        val ques76 = Question(	76,	"What is the synonym of BROWN ?",	"embrown",	"sleeping accommodation",	"mad",	"facial expression",	1
        )
        questionsList.add(ques76)

        val ques77 = Question(	77,	"What is the synonym of BUSY ?",	"occupy",	"supererogatory",	"nicety",	"staff office",	1
        )
        questionsList.add(ques77)

        val ques78 = Question(	78,	"What is the synonym of CAMP ?",	"downcast",	"tooshie",	"encampment",	"notice",	3
        )
        questionsList.add(ques78)

        val ques79 = Question(	79,	"What is the synonym of CAPABLE ?",	"barricade",	"splendid",	"duo",	"equal to",	4
        )
        questionsList.add(ques79)

        val ques80 = Question(	80,	"What is the synonym of CAPITAL ?",	"uppercase",	"let on",	"chum",	"planetary",	1
        )
        questionsList.add(ques80)

        val ques81 = Question(	81,	"What is the synonym of CENTER ?",	"dinero",	"fall",	"centre",	"fag",	3
        )
        questionsList.add(ques81)

        val ques82 = Question(	82,	"What is the synonym of CENTRAL ?",	"mold",	"life",	"lowly",	"exchange",	4
        )
        questionsList.add(ques82)

        val ques83 = Question(	83,	"What is the synonym of CHAMPION ?",	"hero",	"Sami",	"take away",	"detonate",	1
        )
        questionsList.add(ques83)

        val ques84 = Question(	84,	"What is the synonym of CHANCE ?",	"round out",	"menstruum",	"gamble",	"accompany",	3
        )
        questionsList.add(ques84)

        val ques85 = Question(	85,	"What is the synonym of CHANGING ?",	"grade",	"lucre",	"zillion",	"change",	4
        )
        questionsList.add(ques85)

        val ques86 = Question(	86,	"What is the synonym of CHEAP ?",	"white potato vine",	"pep up",	"inexpensive",	"once again",	3
        )
        questionsList.add(ques86)

        val ques87 = Question(	87,	"What is the synonym of CHEMICAL ?",	"fundament",	"chemical substance",	"or so",	"entering",	2
        )
        questionsList.add(ques87)

        val ques88 = Question(	88,	"What is the synonym of CHICKEN ?",	"tawdry",	"father",	"poulet",	"Teach",	3
        )
        questionsList.add(ques88)

        val ques89 = Question(	89,	"What is the synonym of CHIEF ?",	"try",	"tilt",	"head",	"preindication",	3
        )
        questionsList.add(ques89)

        val ques90 = Question(	90,	"What is the synonym of CHOICE ?",	"consistence",	"pick",	"account",	"particularly",	2
        )
        questionsList.add(ques90)

        val ques91 = Question(	91,	"What is the synonym of CLASSIC ?",	"make",	"attach",	"focus on",	"Greco-Roman",	4
        )
        questionsList.add(ques91)

        val ques92 = Question(	92,	"What is the synonym of CLEAN ?",	"pig",	"fight back",	"by",	"clean and jerk",	4
        )
        questionsList.add(ques92)

        val ques93 = Question(	93,	"What is the synonym of CLEAR ?",	"favorable",	"zone",	"chase after",	"open",	4
        )
        questionsList.add(ques93)

        val ques94 = Question(	94,	"What is the synonym of CLOSE ?",	"suck up",	"renowned",	"quotidian",	"finish",	4
        )
        questionsList.add(ques94)

        val ques95 = Question(	95,	"What is the synonym of COLD ?",	"low temperature",	"cut",	"mankind",	"black pepper",	1
        )
        questionsList.add(ques95)

        val ques96 = Question(	96,	"What is the synonym of COLLECT ?",	"first",	"roll up",	"split up",	"cue",	2
        )
        questionsList.add(ques96)

        val ques97 = Question(	97,	"What is the synonym of COLLECTIVE ?",	"corporate",	"hinder",	"affiliate",	"check",	1
        )
        questionsList.add(ques97)

        val ques98 = Question(	98,	"What is the synonym of COLONIAL ?",	"result",	"shiner",	"clause",	"compound",	4
        )
        questionsList.add(ques98)

        val ques99 = Question(	99,	"What is the synonym of COLOR ?",	"wear down",	"hanker",	"colour",	"IT",	3
        )
        questionsList.add(ques99)

        val ques100 = Question(	100,	"What is the synonym of COMFORTABLE ?",	"assemble",	"restate",	"comfy",	"K",	3
        )
        questionsList.add(ques100)

        val ques101 = Question(	101,	"What is the synonym of COMMERCIAL ?",	"good",	"coffin nail",	"commercial message",	"G",	3
        )
        questionsList.add(ques101)

        val ques102 = Question(	102,	"What is the synonym of COMMON ?",	"park",	"grade",	"boot",	"press",	1
        )
        questionsList.add(ques102)

        val ques103 = Question(	103,	"What is the synonym of COMPETITIVE ?",	"split up",	"weary",	"take on",	"competitory",	4
        )
        questionsList.add(ques103)

        val ques104 = Question(	104,	"What is the synonym of COMPLETE ?",	"ontogeny",	"fill in",	"take",	"come along",	2
        )
        questionsList.add(ques104)

        val ques105 = Question(	105,	"What is the synonym of COMPLICATED ?",	"dab",	"complicate",	"discount",	"interbreed",	2
        )
        questionsList.add(ques105)

        val ques106 = Question(	106,	"What is the synonym of COMPREHENSIVE ?",	"comprehensive examination",	"prescription medicine",	"civilize",	"forgather",	1
        )
        questionsList.add(ques106)

        val ques107 = Question(	107,	"What is the synonym of CONCERNED ?",	"refer",	"deplumate",	"unbalanced",	"presently",	1
        )
        questionsList.add(ques107)

        val ques108 = Question(	108,	"What is the synonym of CONFIDENT ?",	"louse up",	"fix",	"convinced",	"essential",	3
        )
        questionsList.add(ques108)

        val ques109 = Question(	109,	"What is the synonym of CONSERVATIVE ?",	"attestator",	"materialistic",	"all-encompassing",	"shortly",	2
        )
        questionsList.add(ques109)

        val ques110 = Question(	110,	"What is the synonym of CONSISTENT ?",	"logical",	"whirlybird",	"deal",	"onwards",	1
        )
        questionsList.add(ques110)

        val ques111 = Question(	111,	"What is the synonym of CONSTANT ?",	"onward",	"lookout man",	"civilization",	"constant quantity",	4
        )
        questionsList.add(ques111)

        val ques112 = Question(	112,	"What is the synonym of CONSTITUTIONAL ?",	"built-in",	"premise",	"in the lead",	"deliver",	1
        )
        questionsList.add(ques112)

        val ques113 = Question(	113,	"What is the synonym of CONTEMPORARY ?",	"large",	"repay",	"coeval",	"feeling",	3
        )
        questionsList.add(ques113)

        val ques114 = Question(	114,	"What is the synonym of CONTENT ?",	"charm",	"clean-handed",	"betray",	"message",	4
        )
        questionsList.add(ques114)

        val ques115 = Question(	115,	"What is the synonym of CONTINUED ?",	"continue",	"learnedness",	"verbalise",	"nightfall",	1
        )
        questionsList.add(ques115)

        val ques116 = Question(	116,	"What is the synonym of COOL ?",	"sire",	"sour",	"aplomb",	"leave off",	3
        )
        questionsList.add(ques116)

        val ques117 = Question(	117,	"What is the synonym of CORPORATE ?",	"work",	"bodied",	"localise",	"malicious gossip",	2
        )
        questionsList.add(ques117)

        val ques118 = Question(	118,	"What is the synonym of CORRECT ?",	"flavor",	"aged",	"rectify",	"dish out",	3
        )
        questionsList.add(ques118)

        val ques119 = Question(	119,	"What is the synonym of CORRESPONDENT ?",	"noetic",	"newspaperwoman",	"ethical code",	"come on",	2
        )
        questionsList.add(ques119)

        val ques120 = Question(	120,	"What is the synonym of COUNTER ?",	"retort",	"extremely",	"blackguard",	"take on",	1
        )
        questionsList.add(ques120)

        val ques121 = Question(	121,	"What is the synonym of CRACK ?",	"give chase",	"tarradiddle",	"cleft",	"snitch",	3
        )
        questionsList.add(ques121)

        val ques122 = Question(	122,	"What is the synonym of CRAZY ?",	"substance",	"work",	"nutcase",	"wide-cut",	3
        )
        questionsList.add(ques122)

        val ques123 = Question(	123,	"What is the synonym of CREATIVE ?",	"overwinter",	"promote",	"originative",	"flattop",	3
        )
        questionsList.add(ques123)

        val ques124 = Question(	124,	"What is the synonym of CRIMINAL ?",	"crook",	"dough",	"ascension",	"drop off",	1
        )
        questionsList.add(ques124)

        val ques125 = Question(	125,	"What is the synonym of CROSS ?",	"chapeau",	"high mallow",	"crisscross",	"bottom",	3
        )
        questionsList.add(ques125)

        val ques126 = Question(	126,	"What is the synonym of CRUCIAL ?",	"confidential information",	"valuate",	"direful",	"all-important",	4
        )
        questionsList.add(ques126)

        val ques127 = Question(	127,	"What is the synonym of CULTURAL ?",	"work up",	"occupy",	"ethnical",	"federal agency",	3
        )
        questionsList.add(ques127)

        val ques128 = Question(	128,	"What is the synonym of CURIOUS ?",	"blanched",	"pose",	"snap",	"funny",	4
        )
        questionsList.add(ques128)

        val ques129 = Question(	129,	"What is the synonym of CURRENT ?",	"flow",	"exemplify",	"research worker",	"lowly",	1
        )
        questionsList.add(ques129)

        val ques130 = Question(	130,	"What is the synonym of CUSTOM ?",	"foot",	"hebdomadal",	"ascension",	"usage",	4
        )
        questionsList.add(ques130)

        val ques131 = Question(	131,	"What is the synonym of CUT ?",	"shit",	"roll",	"demand",	"slash",	4
        )
        questionsList.add(ques131)

        val ques132 = Question(	132,	"What is the synonym of DAILY ?",	"tush",	"generate",	"view",	"day-by-day",	4
        )
        questionsList.add(ques132)

        val ques133 = Question(	133,	"What is the synonym of DANGEROUS ?",	"tenuous",	"aerofoil",	"unsafe",	"automobile",	3
        )
        questionsList.add(ques133)

        val ques134 = Question(	134,	"What is the synonym of DARK ?",	"abuse",	"level",	"recognise",	"wickedness",	4
        )
        questionsList.add(ques134)

        val ques135 = Question(	135,	"What is the synonym of DEAD ?",	"beat",	"shopping center",	"vesture",	"science lab",	1
        )
        questionsList.add(ques135)

        val ques136 = Question(	136,	"What is the synonym of DEAR ?",	"curious",	"beloved",	"musing",	"utilisation",	2
        )
        questionsList.add(ques136)

        val ques137 = Question(	137,	"What is the synonym of DEEP ?",	"hybridize",	"trench",	"wage",	"ballot",	2
        )
        questionsList.add(ques137)

        val ques138 = Question(	138,	"What is the synonym of DEFENSIVE ?",	"spliff",	"defensive attitude",	"postulate",	"in the lead",	2
        )
        questionsList.add(ques138)

        val ques139 = Question(	139,	"What is the synonym of DEMOCRATIC ?",	"articulatio genus",	"give",	"Democratic",	"hoi polloi",	3
        )
        questionsList.add(ques139)

        val ques140 = Question(	140,	"What is the synonym of DEPENDENT ?",	"construct",	"handsome",	"dependant",	"sitting",	3
        )
        questionsList.add(ques140)

        val ques141 = Question(	141,	"What is the synonym of DETAILED ?",	"workings",	"hatful",	"elaborate",	"bump off",	3
        )
        questionsList.add(ques141)

        val ques142= Question(	142,	"What is the synonym of DEVELOPING ?",	"holiday",	"exit",	"development",	"gist",	3
        )
        questionsList.add(ques142)

        val ques143 = Question(	143,	"What is the synonym of DIFFERENT ?",	"dissimilar",	"utterly",	"account",	"R-2",	1
        )
        questionsList.add(ques143)

        val ques144 = Question(	144,	"What is the synonym of DIRECT ?",	"target",	"trigger off",	"full moon",	"consort",	1
        )
        questionsList.add(ques144)

        val ques145 = Question(	145,	"What is the synonym of DIRT ?",	"severalise",	"soil",	"specialise",	"outcry",	2
        )
        questionsList.add(ques145)

        val ques146 = Question(	146,	"What is the synonym of DIRTY ?",	"exercise",	"soil",	"mill",	"opt",	2
        )
        questionsList.add(ques146)

        val ques147 = Question(	147,	"What is the synonym of DISTANT ?",	"find out",	"usance",	"remote",	"call for",	3
        )
        questionsList.add(ques147)


        val ques148 = Question(	148,	"What is the synonym of DOMESTIC ?",	"alternate",	"buck private",	"percipient",	"domestic help",	4
        )
        questionsList.add(ques148)

        val ques149 = Question(	149,	"What is the synonym of DOMINANT ?",	"deepness",	"mention",	"agone",	"dominant allele",	4
        )
        questionsList.add(ques149)

        val ques150 = Question(	150,	"What is the synonym of DOUBLE ?",	"two-base hit",	"clip",	"muckle",	"free-enterprise",	1
        )
        questionsList.add(ques150)

        val ques151= Question(	151,	"What is the synonym of DOWN ?",	"move up",	"bolt down",	"involve",	"lip",	2
        )
        questionsList.add(ques151)

        val ques152 = Question(	152,	"What is the synonym of DOWNTOWN ?",	"business district",	"frighten away",	"screaming",	"rouse",	1
        )
        questionsList.add(ques152)

        val ques153 = Question(	153,	"What is the synonym of DOZEN ?",	"shopping center",	"draw",	"white pepper",	"twelve",	4
        )
        questionsList.add(ques153)

        val ques154 = Question(	154,	"What is the synonym of DRAMATIC ?",	"belt down",	"transport",	"spectacular",	"distaff",	3
        )
        questionsList.add(ques154)

        val ques155 = Question(	155,	"What is the synonym of DRESS ?",	"primp",	"wearing apparel",	"hoops",	"sullen",	2
        )
        questionsList.add(ques155)

        val ques156 = Question(	156,	"What is the synonym of DRY ?",	"iii",	"gravy",	"ironic",	"round down",	3
        )
        questionsList.add(ques156)

        val ques157 = Question(	157,	"What is the synonym of DUE ?",	"ascribable",	"jolting",	"nativity",	"retrieve",	1
        )
        questionsList.add(ques157)

        val ques158 = Question(	158,	"What is the synonym of EACH ?",	"sassing",	"tawdry",	"to each one",	"Holocene",	3
        )
        questionsList.add(ques158)

        val ques159 = Question(	159,	"What is the synonym of EAGER ?",	"automobile",	"hollo",	"bore",	"unequaled",	3
        )
        questionsList.add(ques159)

        val ques160 = Question(	160,	"What is the synonym of EARLY ?",	"get in",	"divide",	"food grain",	"other",	4
        )
        questionsList.add(ques160)

        val ques161 = Question(	161,	"What is the synonym of DISTINCT ?",	"comforter",	"cite",	"valet de chambre",	"trenchant",	4
        )
        questionsList.add(ques161)

//        val ques = Question()
//        questionsList.add(ques)









        return questionsList
    }
}