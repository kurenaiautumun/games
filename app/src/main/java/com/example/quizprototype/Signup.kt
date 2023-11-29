package com.example.quizprototype

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import com.google.firebase.FirebaseException
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.PhoneAuthCredential
import com.google.firebase.auth.PhoneAuthOptions
import com.google.firebase.auth.PhoneAuthProvider
import java.util.concurrent.TimeUnit

class Signup : AppCompatActivity() {
    private  lateinit var mauth: FirebaseAuth
    private lateinit var mobileNo : EditText
    private lateinit var uname :EditText
    private lateinit var password :EditText
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_signup)


        val already = findViewById<TextView>(R.id.Already)
        val register = findViewById<Button>(R.id.buttonreg)
        uname = findViewById(R.id.usernamereg)
        password = findViewById(R.id.Passwordreg)
        mobileNo = findViewById(R.id.regPhone)
        val number = mobileNo.text
        mauth = FirebaseAuth.getInstance()


        register.setOnClickListener {
//            Toast.makeText(this,number.toString(),Toast.LENGTH_LONG).show()
            if(uname.text.isNotEmpty() && password.text.isNotEmpty() && mobileNo.text.isNotEmpty()){
                if(number.toString().length==10){
                    startverification()
                }
                else{
                    Toast.makeText(this,"Invaild Phone Number",Toast.LENGTH_LONG).show()
                }
            }
            else{
                Toast.makeText(this,"Details not filled",Toast.LENGTH_LONG).show()
            }
        }

        already.setOnClickListener {
            startActivity(Intent(this,Login::class.java))
            finish()
        }
    }

    private fun startverification() {
        val callbacks = object : PhoneAuthProvider.OnVerificationStateChangedCallbacks() {
            override fun onVerificationCompleted(credential: PhoneAuthCredential) {

            }

            override fun onVerificationFailed(e: FirebaseException) {
                Toast.makeText(this@Signup,e.localizedMessage.toString(), Toast.LENGTH_LONG).show()
                Log.d("resp",e.localizedMessage.toString())
            }

            override fun onCodeSent(
                verificationId: String,
                token: PhoneAuthProvider.ForceResendingToken
            ) {
                val tempuser = uname.text
                val temppass = password.text
                val tempnum = mobileNo.text
                val intent = Intent(this@Signup,OTP::class.java)
                intent.putExtra("verification",verificationId)
                intent.putExtra("user",tempuser.toString())
                intent.putExtra("password",temppass.toString())
                intent.putExtra("phone",tempnum.toString())
                startActivity(intent)
            }
        }

        val options = PhoneAuthOptions.newBuilder(mauth)
            .setPhoneNumber("+91"+mobileNo.text.toString().trim())       // Phone number to verify
            .setTimeout(60L, TimeUnit.SECONDS) // Timeout duration
            .setActivity(this)                 // Activity (for callback binding)
            .setCallbacks(callbacks)            // OnVerificationStateChangedCallbacks
            .build()

        PhoneAuthProvider.verifyPhoneNumber(options)
    }
}