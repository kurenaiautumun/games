package com.example.quizprototype.Adapter

import android.content.Context
import android.content.Intent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.quizprototype.DataClasses.Science
import com.example.quizprototype.Quizing
import com.example.quizprototype.R
import java.util.*

class HorizontalAdapter(private val context: Context, private val items: List<Science>, private val category:String) :
    RecyclerView.Adapter<HorizontalAdapter.ViewHolder>() {

    class ViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val imageView: ImageView = itemView.findViewById(R.id.imageView)
        var name = itemView.findViewById<TextView>(R.id.textView)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.item_view, parent, false)
        return ViewHolder(view)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val item = items[position]

        val modifiedname = item.name.substringBefore(".")
        holder.name.text = modifiedname.uppercase(Locale.ROOT)
        // Use an image loading library like Glide to load the image
        Glide.with(holder.itemView.context)
            .load(item.imageUri)
            .into(holder.imageView)


        holder.itemView.setOnClickListener{
            val intent = Intent(context, Quizing::class.java)
            intent.putExtra("category",category)
            intent.putExtra("underSection",modifiedname)
            context.startActivity(intent)
        }
    }

    override fun getItemCount(): Int {
        return items.size
    }
}