package com.example.quizprototype

import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.widget.Toast

class SplashScreen : AppCompatActivity() {
    private lateinit var sharedPreferences: SharedPreferences
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_splash_screen)

        sharedPreferences = getSharedPreferences("MyPrefs", Context.MODE_PRIVATE)

        val saveuid = sharedPreferences.getString("UID", "")
        supportActionBar?.hide()
        Handler(Looper.getMainLooper()).postDelayed({
            if(saveuid!=""){
//                Toast.makeText(this,saveuid.toString(),Toast.LENGTH_LONG).show()
                startActivity(Intent(this,BaseActivity::class.java))
                finish()
            }
            else {
                startActivity(Intent(this, Signup::class.java))
                finish()
            }
        },2000)


    }
}