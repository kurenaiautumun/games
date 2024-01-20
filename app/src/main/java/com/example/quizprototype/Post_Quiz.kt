package com.example.quizprototype

import android.content.Context
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.CountDownTimer
import android.os.Handler
import android.os.Looper
import android.util.Log
import android.widget.Button
import android.widget.RadioButton
import android.widget.RadioGroup
import android.widget.TextView
import android.widget.Toast
import com.example.quizprototype.DataClasses.modifiedquestions
import com.example.quizprototype.DataClasses.stored_modifiedquestions
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken

class Post_Quiz : AppCompatActivity() {

    private lateinit var optionsRadioGroup : RadioGroup
    private lateinit var  submitButton : Button
    private lateinit var questionTextView: TextView
    private lateinit var option1RadioButton: RadioButton
    private lateinit var option2RadioButton: RadioButton
    private lateinit var option3RadioButton: RadioButton
    private lateinit var option4RadioButton: RadioButton
    private lateinit var result : TextView
    private lateinit var timerView : TextView
    private lateinit var questionlist : MutableList<modifiedquestions>
    private lateinit var storedlist : MutableList<stored_modifiedquestions>
    private lateinit var misslist : MutableList<modifiedquestions>
    private lateinit var Wronglist:MutableList<modifiedquestions>
    private lateinit var titleSection : String

    private var currentQuestionIndex = 0
    private var score = 0
    private var timer: CountDownTimer? = null

    val totalCategories = 0
    var completedCategories = 0

    private val handler = Handler(Looper.getMainLooper())

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_post_test)

        optionsRadioGroup = findViewById(R.id.optionsRadioGroupPOST)
        submitButton = findViewById(R.id.submitButtonPost)
        result = findViewById(R.id.resultTextViewPost)
        questionTextView = findViewById(R.id.questionTextViewPost)
        option1RadioButton = findViewById(R.id.optionARadioButtonPost)
        option2RadioButton = findViewById(R.id.optionBRadioButtonPost)
        option3RadioButton = findViewById(R.id.optionCRadioButtonPost)
        option4RadioButton = findViewById(R.id.optionDRadioButtonPost)
        timerView = findViewById(R.id.TimerViewPost)

        questionlist = mutableListOf()
        misslist = mutableListOf()
        Wronglist = mutableListOf()

        val sets = intent.getStringExtra("section")
        titleSection = intent.getStringExtra("category").toString()

        val sharedPreferences = getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        val jsonString = sharedPreferences.getString(sets, "")

//        Toast.makeText(this,sets,Toast.LENGTH_LONG).show()
        // Convert JSON string back to List<ScienceData>
         storedlist = Gson().fromJson(jsonString, object : TypeToken<List<stored_modifiedquestions>>() {}.type)

        val index = storedlist.indexOfFirst { it.title == titleSection}
        if(index!=-1) {

            questionlist = storedlist[index].subsections.toMutableList()
//            Toast.makeText(this,questionlist.size.toString(),Toast.LENGTH_LONG).show()
        }


        if(questionlist.size>0) {
            display_Question()
        }
        submitButton.setOnClickListener {
            processing_question(sets)
        }

//        Log.d("list",questionlist.toString())
    }


    private fun display_Question() {
        result.text = ""

        val question = questionlist[currentQuestionIndex]
        questionTextView.text = "Q.${currentQuestionIndex + 1} " + question.question
        option1RadioButton.text = question.option1
        option2RadioButton.text = question.option2
        option3RadioButton.text = question.option3
        option4RadioButton.text = question.Option4

        optionsRadioGroup.clearCheck()

        startTimer(11)
    }

    private fun processing_question(sets: String?) {

        timer?.cancel()
        val selectedRadioButtonId = optionsRadioGroup.checkedRadioButtonId
        currentQuestionIndex++
        val question_object = questionlist[currentQuestionIndex-1]
        if (selectedRadioButtonId != -1) {
            val selectedRadioButton = findViewById<RadioButton>(selectedRadioButtonId)
            val selectedAnswer = selectedRadioButton.text.toString()
            // Check the selected answer and perform further actions
            if(checkanswer(selectedAnswer)) {
                result.text = "Correct Answer"
                result.setTextColor(getColor(R.color.Correct))
                questionlist[currentQuestionIndex-1].tries -=1
                score++
            }
            else {
                result.text = "Wrong Answer"
                questionlist[currentQuestionIndex-1].tries +=1
                result.setTextColor(getColor(R.color.Incorrect))
            }

        } else {
            // No answer selected
                result.text = "Wrong Answer"
                questionlist[currentQuestionIndex-1].tries +=1
                result.setTextColor(getColor(R.color.Incorrect))

        }



        if(currentQuestionIndex<questionlist.size){

            handler.postDelayed({
                display_Question()
//                startTimer(10)
            }, 500) // Delay for 0.5 seconds (500 milliseconds)

        }
        else{
            if(sets=="wrong")
            saveData(questionlist,"wrong")
            else
            saveData(questionlist,"miss")
            val intent = Intent(this, Result::class.java)
            intent.putExtra("score", score.toString())
            intent.putExtra("Quescount",questionlist.size.toString())
            startActivity(intent)
            finish()
        }

    }

    fun saveData(list: MutableList<modifiedquestions>, saveString: String) {
        val sharedPreferences = getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)
        val editor = sharedPreferences.edit()

        // Retrieve existing data from SharedPreferences
        val gson = Gson()
        val existingJson = sharedPreferences.getString(saveString, "")
        val existingListType = object : TypeToken<List<stored_modifiedquestions>>() {}.type
        val existingList: MutableList<stored_modifiedquestions> =
            gson.fromJson(existingJson, existingListType) ?: mutableListOf()

        // Find the section in the existing list
        val existingSectionIndex = existingList.indexOfFirst { it.title == titleSection }

        list.removeIf{it.tries==0}

        var existingSection = list
        existingList[existingSectionIndex] = stored_modifiedquestions(titleSection, existingSection)

        val updatedJson = gson.toJson(existingList)
        editor.putString(saveString, updatedJson)
        editor.apply()
    }

    private fun checkanswer(selectedAnswer: String): Boolean {
        val correctAnswer = questionlist[currentQuestionIndex-1].answer

        return selectedAnswer == correctAnswer
    }

    private fun startTimer(seconds: Long) {
        timer = object : CountDownTimer(seconds * 1000, 1000) {
            override fun onTick(millisUntilFinished: Long) {
                // Update the timer display with the remaining time
                timerView.text = "Time remaining: ${millisUntilFinished / 1000} seconds"
            }

            override fun onFinish() {
                currentQuestionIndex++
                if (currentQuestionIndex < questionlist.size) {
                    Toast.makeText(this@Post_Quiz,questionlist.size,Toast.LENGTH_LONG).show()

                    display_Question()
                }
                else{


                    val intent = Intent(this@Post_Quiz, Result::class.java)
                        intent.putExtra("score", score.toString())
                        intent.putExtra("Quescount",questionlist.size.toString())
                        startActivity(intent)
                        finish()
                }

            }
        }.start()
    }
    override fun onDestroy() {
        // Cancel the timer when the activity is destroyed
        timer?.cancel()
        super.onDestroy()
    }
}