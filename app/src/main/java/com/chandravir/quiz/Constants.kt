package com.chandravir.quiz

object Constants {

    const val USER_NAME: String = "user_name"
    const val TOTAL_QUESTIONS: String = "total_question"
    const val CORRECT_ANSWERS: String = "correct_answers"

    fun getQuestions(): ArrayList<Question>{
        val questionsList = ArrayList<Question>()

            val ques1 = Question(1,
                "Who owns the ANDROID company?",
                R.drawable.android,
            "Google",
                "Apple",
                "Amazon",
                "Microsoft",
                1,
            )

        questionsList.add(ques1)

        val ques2 = Question(1,
            "Which company owns the Safari browser?",
            R.drawable.safari,
            "Apple",
            "Google",
            "Microsoft",
            "Amazon",
            1
        )

        questionsList.add(ques2)

        val ques3 = Question(1,
            "Where should we upload the apk file?",
            R.drawable.playstore,
            "Microsoft Store",
            "App Store",
            "Google Drive",
            "Play Store",
            4,
        )

        questionsList.add(ques3)

        val ques4 = Question(1,
            "Who owns the APPLE company?",
            R.drawable.apple,
            "Google",
            "Apple",
            "Amazon",
            "Microsoft",
            2,
        )

        questionsList.add(ques4)

        val ques5 = Question(1,
            "Who is CEO of google?",
            R.drawable.google,
            "Satya Nadella",
            "Jeff Bezos",
            "Tim Cook",
            "Sundar Pichai",
            4,
        )

        questionsList.add(ques5)

        return questionsList
    }
}