package com.example.quizprototype

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import com.google.firebase.FirebaseException
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.PhoneAuthCredential
import com.google.firebase.auth.PhoneAuthOptions
import com.google.firebase.auth.PhoneAuthProvider
import java.util.concurrent.TimeUnit

class PhoneLogin : AppCompatActivity() {
    private  lateinit var mauth:FirebaseAuth
    private lateinit var phone : EditText
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_phone_login)

        val sendbutton = findViewById<Button>(R.id.sendbutton)
        phone = findViewById(R.id.phonenumber)

        mauth = FirebaseAuth.getInstance()
        sendbutton.setOnClickListener {
//            Toast.makeText(this,phone.text.toString(),Toast.LENGTH_LONG).show()
            if(phone.text.toString().length!=10){
                Toast.makeText(this,"Invailid Phone Number",Toast.LENGTH_LONG).show()
            }
            else {
                startverification()
            }
        }

    }

    private fun startverification() {
        val callbacks = object : PhoneAuthProvider.OnVerificationStateChangedCallbacks() {
            override fun onVerificationCompleted(credential: PhoneAuthCredential) {

            }

            override fun onVerificationFailed(e: FirebaseException) {
                Toast.makeText(this@PhoneLogin,e.localizedMessage,Toast.LENGTH_LONG).show()
                Log.d("resp",e.localizedMessage.toString())
            }

            override fun onCodeSent(
                verificationId: String,
                token: PhoneAuthProvider.ForceResendingToken
            ) {
                val intent = Intent(this@PhoneLogin,OTP::class.java)
                intent.putExtra("verification",verificationId)
                startActivity(intent)
            }
        }

        val options = PhoneAuthOptions.newBuilder(mauth)
            .setPhoneNumber("+91"+phone.text.toString().trim())       // Phone number to verify
            .setTimeout(60L, TimeUnit.SECONDS) // Timeout duration
            .setActivity(this)                 // Activity (for callback binding)
            .setCallbacks(callbacks)            // OnVerificationStateChangedCallbacks
            .build()

        PhoneAuthProvider.verifyPhoneNumber(options)
    }
}