package com.chandravir.quiz

import android.annotation.SuppressLint
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.telephony.SmsMessage.SubmitPdu
import android.view.View
import android.widget.TextView
import androidx.cardview.widget.CardView
import com.chandravir.quiz.R.id.submitCard

class ResultActivity : AppCompatActivity() {

    lateinit var txtUsername: TextView
    lateinit var txtScore: TextView
    lateinit var submitCard: CardView
    lateinit var txtSubmit : TextView


    @SuppressLint("MissingInflatedId")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_result)

        window.decorView.systemUiVisibility = View.SYSTEM_UI_FLAG_FULLSCREEN

        txtUsername = findViewById(R.id.txtUsername)
        txtScore = findViewById(R.id.txtScore)
        submitCard = findViewById(R.id.submitCard)
        txtSubmit = findViewById(R.id.txtSubmit)

        val username = intent.getStringExtra(Constants.USER_NAME)
        txtUsername.text = username

        val correctAnswers = intent.getIntExtra(Constants.CORRECT_ANSWERS, 0)

        txtScore.text = "Your Score is $correctAnswers"

        txtSubmit.setOnClickListener {
            startActivity(Intent(this, MainActivity::class.java))
        }


    }
}