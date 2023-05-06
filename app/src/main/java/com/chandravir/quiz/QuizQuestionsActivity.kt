package com.chandravir.quiz

import android.content.Intent
import android.graphics.Color
import android.graphics.Typeface
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.view.View.OnClickListener
import android.widget.*
import androidx.core.content.ContextCompat

class QuizQuestionsActivity : AppCompatActivity(), OnClickListener {

    private lateinit var txtQuestion: TextView
    private lateinit var imgQuestion: ImageView
    private lateinit var progressBar: ProgressBar
    private lateinit var txtProgressIndicator: TextView
    private lateinit var txtOptionOne: TextView
    private lateinit var txtOptionTwo: TextView
    private lateinit var txtOptionThree: TextView
    private lateinit var txtOptionFour: TextView
    private lateinit var btnSubmit: Button

    private var mCurrentPosition: Int = 1
    private var mQuestionsList: ArrayList<Question>? = null
    private var mSelectedOptionPosition: Int = 0

    private var mCorrectAnswers: Int = 0

    private var mUserName: String? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_quiz_questions)

        mUserName = intent.getStringExtra(Constants.USER_NAME)

        txtQuestion = findViewById(R.id.txtQuestion)
        imgQuestion = findViewById(R.id.imgQuestion)
        progressBar = findViewById(R.id.progressBar)
        txtProgressIndicator = findViewById(R.id.txtProgressIndicator)
        txtOptionOne = findViewById(R.id.txtOptionOne)
        txtOptionTwo = findViewById(R.id.txtOptionTwo)
        txtOptionThree = findViewById(R.id.txtOptionThree)
        txtOptionFour = findViewById(R.id.txtOptionFour)
        btnSubmit = findViewById(R.id.btnSubmit)

        mQuestionsList = Constants.getQuestions()
        setQuestion()

        txtOptionOne.setOnClickListener(this)
        txtOptionTwo.setOnClickListener(this)
        txtOptionThree.setOnClickListener(this)
        txtOptionFour.setOnClickListener(this)
        btnSubmit.setOnClickListener(this)

    }

    private fun setQuestion(){


        val question = mQuestionsList!![mCurrentPosition -1]

        defaultOptionsView()

        if(mCurrentPosition == mQuestionsList!!.size){
            btnSubmit.text = "FINISH"
        }
        else{
            btnSubmit.text = "SUBMIT"
        }

        progressBar.progress = mCurrentPosition
        txtProgressIndicator.text = "$mCurrentPosition" + "/" + progressBar.max

        txtQuestion.text = question!!.question
        imgQuestion.setImageResource(question.image)
        txtOptionOne.text = question.optionOne
        txtOptionTwo.text = question.optionTwo
        txtOptionThree.text = question.optionThree
        txtOptionFour.text = question.optionFour
    }

    private fun defaultOptionsView(){
        val options = ArrayList<TextView>()
        options.add(0, txtOptionOne)
        options.add(1, txtOptionTwo)
        options.add(2, txtOptionThree)
        options.add(3, txtOptionFour)

        for(option in options){
            option.setTextColor(Color.parseColor("#7A8089"))
            option.typeface = Typeface.DEFAULT
            option.background  = ContextCompat.getDrawable(
                this, R.drawable.option_background
            )

        }

    }

    override fun onClick(v: View?) {

        when(v?.id){
            R.id.txtOptionOne -> {selectedOptionView(txtOptionOne, 1)}
            R.id.txtOptionTwo -> {selectedOptionView(txtOptionTwo, 2)}
            R.id.txtOptionThree -> {selectedOptionView(txtOptionThree, 3)}
            R.id.txtOptionFour -> {selectedOptionView(txtOptionFour, 4)}
            R.id.btnSubmit -> {
                if (mSelectedOptionPosition == 0){
                    mCurrentPosition ++

                    when{
                        mCurrentPosition <= mQuestionsList!!.size ->{
                        setQuestion()
                    }
                        else -> {
                            val intent = Intent(this, ResultActivity::class.java)
                            intent.putExtra(Constants.USER_NAME , mUserName)
                            intent.putExtra(Constants.CORRECT_ANSWERS, mCorrectAnswers)
                            intent.putExtra(Constants.TOTAL_QUESTIONS, mQuestionsList!!.size)
                            startActivity(intent)
                        }
                    }
                }
                else{
                    val question = mQuestionsList?.get(mCurrentPosition -1)
                    if (question!!.correctAnswer != mSelectedOptionPosition){
                        answerView(mSelectedOptionPosition, R.drawable.wrong_option_background)
                    }
                    else{
                        mCorrectAnswers ++
                    }
                    answerView(question.correctAnswer, R.drawable.correct_option_background)

                    if(mCurrentPosition == mQuestionsList!!.size){
                        btnSubmit.text = "FINISH"
                    }
                    else{
                        btnSubmit.text = "GO TO NEXT QUESTION"
                    }
                    mSelectedOptionPosition = 0
                }
            }
        }

    }

    private fun answerView(answer: Int, drawableView: Int) {
        when (answer) {
            1 -> {
                txtOptionOne.background = ContextCompat.getDrawable(this, drawableView)
            }
            2 -> {
                txtOptionTwo.background = ContextCompat.getDrawable(this, drawableView)
            }
            3 -> {
                txtOptionThree.background = ContextCompat.getDrawable(this, drawableView)
            }
            4 -> {
                txtOptionFour.background = ContextCompat.getDrawable(this, drawableView)
            }
        }
    }

    private fun selectedOptionView(txt: TextView, selectedOptionNumber: Int){
        defaultOptionsView()
        mSelectedOptionPosition = selectedOptionNumber

        txt.setTextColor(Color.parseColor("#363A43"))
        txt.setTypeface(txt.typeface, Typeface.BOLD)
        txt.background  = ContextCompat.getDrawable(
            this, R.drawable.selected_option_background

        )
    }

}